namespace U9.VOB.Cus.HBHJianLiYuan
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
    using UFIDA.U9.CBO.SCM.Warehouse;
    using UFIDA.U9.CBO.HR.Department;
    using UFIDA.U9.PM.Rcv;
    using UFSoft.UBF.Transactions;
    using UFIDA.U9.PM.Enums;

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


    /// <summary>
    /// Impement Implement
    /// 
    /// </summary>	
    internal partial class AQWTransToRcvSVImpementStrategy : BaseStrategy
    {
        //private const string Const_RcvDocTypeCode = "RCV01";
        // 2018-03-07 wf 改为确认审批的可自动审核的单据类型   09   奥琦玮生单
        private const string Const_RcvDocTypeCode = "09";
        // 00222-北京配送部
        private const string Const_SupplierCode = "00222";
        // 00291--北京配送部（冻货）
        private const string Const_FrozenSupplierCode = "00291";

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

            //DataTable dtID = new DataTable("HeadIDs");
            //dtID.Columns.Add("ID", typeof(string));

            //foreach (string id in bpObj.HeadIDs)
            //{

            //}

            {
                // 检查，是否有已生成单据的单子
                string strOutDocIDs = bpObj.HeadIDs.GetOpathFromList("'", "'", ",");

                string strOpath = string.Format("DescFlexField.PrivateDescSeg9 in ({0})"
                    , strOutDocIDs
                    );

                Receivement.EntityList lstRcv = Receivement.Finder.FindAll(strOpath);
                
                if (lstRcv != null
                    && lstRcv.Count > 0
                    )
                {
                    StringBuilder sbOutDocNo = new StringBuilder();
                    StringBuilder sbRcvDocNo = new StringBuilder();

                    foreach (Receivement head in lstRcv)
                    {
                        if (head != null)
                        {
                            sbOutDocNo.Append(head.DescFlexField.PrivateDescSeg2).Append(",");
                            sbRcvDocNo.Append(head.DocNo).Append(",");
                        }
                    }

                    if (sbOutDocNo.Length > 0)
                    {
                        throw new BusinessException(string.Format("外部单据[{0}]已生成收货单[{1}],不允许重复收货!"
                            , sbOutDocNo.GetStringRemoveLastSplit()
                            , sbRcvDocNo.GetStringRemoveLastSplit()
                            ));
                    }
                }
            }


            string strIDs = bpObj.HeadIDs.GetOpathFromIList();

            string strProcName = "HBH_SP_JianLiYuan_GetAQWRcvLineInfo";
            List<ParamDTO> lstParam = new List<ParamDTO>();
            {
                ParamDTO paramDTO = new ParamDTO();
                paramDTO.ParamName = "HeadIDs";
                paramDTO.ParamDirection = ParameterDirection.Input;
                //paramDTO.ParamType = DbType.AnsiString;
                //paramDTO.ParamValue = EntitySerialization.EntitySerial(dtID);
                paramDTO.ParamType = DbType.String;
                paramDTO.ParamValue = strIDs;

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
            // 服务外面包装了事务应该
            //UFIDA.U9.ISV.RCV.CreateRCVSRV creatRcv = new UFIDA.U9.ISV.RCV.CreateRCVSRV();

            //creatRcv.RCVList = new List<UFIDA.U9.ISV.RCV.DTO.OBAReceivementDTO>();
            //foreach (AQWRcvDTO aqwRcvDTO in lstRcvHead)
            //{
            //    OBAReceivementDTO erpRcv = GetErpRcv(aqwRcvDTO);

            //    if (erpRcv != null)
            //    {
            //        creatRcv.RCVList.Add(erpRcv);
            //    }
            //}

            //creatRcv.RCVList = GetRcvList(lstRcvHead);

            //if (creatRcv.RCVList.Count > 0)
            //{
            //    // 多张生单，供应商居然会全部取第一个供应商；集团bug，所以改为每次调用一次生单
            //    Receivement.EntityList lstRcv = creatRcv.Do();

            //    ApproveRcv(lstRcv);
            //}


            // 2018-03-31 多张生单，供应商居然会全部取第一个供应商；集团bug，所以改为每次调用一次生单
            List<OBAReceivementDTO> lstRcvDTO = GetRcvList(lstRcvHead);
            if (lstRcvDTO != null
                && lstRcvDTO.Count > 0
                )
            {
                Receivement.EntityList lstResultRcv = new Receivement.EntityList();

                foreach (OBAReceivementDTO rcvDTO in lstRcvDTO)
                {
                    UFIDA.U9.ISV.RCV.CreateRCVSRV creatRcv = new UFIDA.U9.ISV.RCV.CreateRCVSRV();

                    creatRcv.RCVList = new List<OBAReceivementDTO>();
                    creatRcv.RCVList.Add(rcvDTO);

                    // 多张生单，供应商居然会全部取第一个供应商；集团bug，所以改为每次调用一次生单
                    Receivement.EntityList lstRcv = creatRcv.Do();

                    if (lstRcv != null
                        && lstRcv.Count > 0
                        )
                    {
                        foreach (Receivement head in lstRcv)
                        {
                            lstResultRcv.Add(head);
                        }
                    }
                }

                if (lstResultRcv != null
                    && lstResultRcv.Count > 0
                    )
                {
                    ApproveRcv(lstResultRcv);
                }
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

        private Supplier _rcvFrozenSupplier = null;
        private Supplier RcvFrozenSupplier
        {
            get
            {
                if (_rcvFrozenSupplier == null)
                {
                    _rcvFrozenSupplier = Supplier.Finder.Find("Org=@Org and Code=@Code"
                        , new OqlParam(Context.LoginOrg.ID)
                        , new OqlParam(Const_FrozenSupplierCode)
                        );

                    if (_rcvFrozenSupplier == null)
                    {
                        throw new BusinessException(string.Format("组织[{0}]下没有找到编码为[{1}]的供应商!"
                            , Context.LoginOrg.Name
                            , Const_FrozenSupplierCode
                            ));
                    }
                }
                return _rcvFrozenSupplier;
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
        


        private List<OBAReceivementDTO> GetRcvList(List<AQWRcvDTO> lstAQWRcvHead)
        {
            List<OBAReceivementDTO> lstErpRcv = new List<OBAReceivementDTO>();

            Dictionary<AQWSupplierType, List<AQWRcvLineDTO>> dicRcv = new Dictionary<AQWSupplierType, List<AQWRcvLineDTO>>();
            foreach (AQWRcvDTO rcvDTO in lstAQWRcvHead)
            {
                if (rcvDTO != null
                    && rcvDTO.AQWRcvLineDTOs != null
                    && rcvDTO.AQWRcvLineDTOs.Count > 0
                    )
                {
                    foreach (AQWRcvLineDTO lineDTO in rcvDTO.AQWRcvLineDTOs)
                    {
                        //decimal qty = lineDTO.amount.GetDecimal();
                        decimal qty = lineDTO.TotalQty;
                        if (qty > 0
                            )
                        {
                            AQWSupplierType suptType = AQWSupplierType.Empty;

                            // 当奥琦玮内料号名称前面加“冻货”两个字的，在U9内对应的供应商为“00291--北京配送部（冻货）”，没有冻货两个字的，对应的供应商为“00222-北京配送部”
                            if (lineDTO.lgname.Contains("冻货")
                                // 2018-03-12 wf 冻品也生成冷冻的供应商
                                || lineDTO.lgname.Contains("冻品")
                                )
                            {
                                // supt = RcvFrozenSupplier;
                                suptType = AQWSupplierType.冷冻;
                            }
                            else
                            {
                                //supt = RcvSupplier;
                                suptType = AQWSupplierType.配送;
                            }

                            if (!dicRcv.ContainsKey(suptType))
                            {
                                dicRcv.Add(suptType, new List<AQWRcvLineDTO>());
                            }

                            dicRcv[suptType].Add(lineDTO);
                        }
                    }
                }
            }

            if (dicRcv.Count > 0)
            {
                foreach (AQWSupplierType key in dicRcv.Keys)
                {
                    List<AQWRcvLineDTO> lstAQWRcvLine = dicRcv[key];

                    if (lstAQWRcvLine != null
                        && lstAQWRcvLine.Count > 0
                        )
                    {
                        AQWRcvLineDTO firstAQWRcvLine = lstAQWRcvLine[0];
                        AQWRcvDTO firstAQWRcvHead = firstAQWRcvLine.AQWRcvHead;

                        Supplier supt = null;
                        // 当奥琦玮内料号名称前面加“冻货”两个字的，在U9内对应的供应商为“00291--北京配送部（冻货）”，没有冻货两个字的，对应的供应商为“00222-北京配送部”
                        //if (firstLine.lgname.Contains("冻货"))
                        if(key == AQWSupplierType.冷冻)
                        {
                            supt = RcvFrozenSupplier;
                        }
                        else
                        {
                            supt = RcvSupplier;
                        }


                        StringBuilder sbError = new StringBuilder();

                        OBAReceivementDTO erpRcvHead = new OBAReceivementDTO();

                        RcvDocType rcvDocType = RcvDocType;
                        if (rcvDocType != null)
                        {
                            erpRcvHead.RcvDocType = new UFIDA.U9.PM.DTOs.BESimp4UIDTO();
                            erpRcvHead.RcvDocType.ID = rcvDocType.ID;
                            erpRcvHead.RcvDocType.Code = rcvDocType.Code;
                        }

                        erpRcvHead.BusinessDate = firstAQWRcvHead.arrivetime.GetDateTime(DateTime.Today).Date;


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
                        // wf 2018-03-08 改为行上赋值
                        //erpRcvHead.DescFlexField.PrivateDescSeg1 = aqwRcvDTO.ldiid;
                        //erpRcvHead.DescFlexField.PrivateDescSeg2 = aqwRcvDTO.code;

                        //if (aqwRcvDTO.sno.IsNotNullOrWhiteSpace())
                        //{
                        //    Warehouse wh = Warehouse.Finder.Find("Org=@Org and Code=@Code"
                        //        , new OqlParam(Context.LoginOrg.ID)
                        //        , new OqlParam(aqwRcvDTO.sno)
                        //        );

                        //    if (wh == null)
                        //    {
                        //        throw new BusinessException(string.Format("组织[{0}]下没有找到编码为[{1}]的收货仓库!"
                        //            , Context.LoginOrg.Name
                        //            , aqwRcvDTO.sno
                        //            ));
                        //    }
                        //}

                        // 现在奥琦玮内部门的编码和名称是一个字段，我让他们限定前三位是代码，后面的名称现在在调整成跟U9一致
                        Department dept = null;
                        Warehouse wh = null;

                        string strAqwShopName = firstAQWRcvHead.shopname;
                        if (strAqwShopName.IsNotNullOrWhiteSpace())
                        {
                            string strDept = string.Empty;
                            if (strAqwShopName.Length > 3)
                            {
                                //strDept = strAqwShopName.Substring(0, 3);
                                strDept = strAqwShopName.Substring(3, strAqwShopName.Length - 3);
                            }
                            else
                            {
                                strDept = strAqwShopName;
                            }

                            //dept = Department.Finder.Find("Org=@Org and Code=@Code"
                            dept = Department.Finder.Find("Org=@Org and Name=@Dept"
                                , new OqlParam(Context.LoginOrg.ID)
                                , new OqlParam(strDept)
                                );

                            if (dept == null)
                            {
                                throw new BusinessException(string.Format("组织[{0}]下没有找到名称为[{1}]的部门!"
                                    , Context.LoginOrg.Name
                                    , strDept
                                    ));
                            }
                            //else
                            //{ 

                            //}

                            //string strWhOpath = string.Format("Org=@Org and (Code like '%' + @Code) order by sqlLen(Code) asc,Code asc"
                            //, strDept
                            //);
                            string strWhOpath = string.Format("Org=@Org and Department.Code = @Code order by sqlLen(Code) asc,Code asc");
                            wh = Warehouse.Finder.Find(strWhOpath
                                , new OqlParam(Context.LoginOrg.ID)
                                , new OqlParam(dept.Code)
                                );

                            if (wh == null)
                            {
                                throw new BusinessException(string.Format("组织[{0}]下没有找到所属部门为[{1}]的仓库!"
                                    , Context.LoginOrg.Name
                                    , dept.Code
                                    ));
                            }
                        }
                        else
                        {
                            throw new BusinessException(string.Format("单据[{0}]店铺不可为空!"
                                , firstAQWRcvHead.code
                                ));
                        }
                        
                        List<string> lstAQWID = new List<string>();
                        List<string> lstAQWDocNo = new List<string>();

                        erpRcvHead.RcvLines = new List<OBARcvLineDTO>();
                        foreach (AQWRcvLineDTO aqwRcvLineDTO in lstAQWRcvLine)
                        {
                            if (aqwRcvLineDTO != null)
                            {
                                OBARcvLineDTO erpRcvLine = GetErpRcvLine(aqwRcvLineDTO, erpRcvHead, dept, wh);

                                if (erpRcvLine != null)
                                {
                                    erpRcvHead.RcvLines.Add(erpRcvLine);

                                    AQWRcvDTO aqwRcvHeadDTO = aqwRcvLineDTO.AQWRcvHead;

                                    if (!lstAQWID.Contains(aqwRcvHeadDTO.ldiid))
                                    {
                                        lstAQWID.Add(aqwRcvHeadDTO.ldiid);
                                    }
                                    if (!lstAQWDocNo.Contains(aqwRcvHeadDTO.code))
                                    {
                                        lstAQWDocNo.Add(aqwRcvHeadDTO.code);
                                    }
                                }
                            }
                        }
                        
                        // wf 2018-03-08 改为行上赋值
                        //erpRcvHead.DescFlexField.PrivateDescSeg1 = aqwRcvDTO.ldiid;
                        //erpRcvHead.DescFlexField.PrivateDescSeg2 = aqwRcvDTO.code;

                        erpRcvHead.DescFlexField.PrivateDescSeg1 = lstAQWID.GetOpathFromIList();
                        erpRcvHead.DescFlexField.PrivateDescSeg2 = lstAQWDocNo.GetOpathFromIList();

                        if (sbError.Length > 0)
                        {
                            throw new BusinessException(sbError.ToString());
                        }

                        // 有行，才返回；没行没意义；
                        if (erpRcvHead.RcvLines.Count > 0)
                        {
                            lstErpRcv.Add(erpRcvHead);
                        }
                        else
                        {
                            //return null;
                            continue;
                        }
                    }
                }
            }

            return lstErpRcv;
        }


        private OBAReceivementDTO GetErpRcv(AQWRcvDTO aqwRcvDTO)
        {
            if (aqwRcvDTO != null
                && aqwRcvDTO.AQWRcvLineDTOs != null
                && aqwRcvDTO.AQWRcvLineDTOs.Count > 0
                )
            {
                AQWRcvLineDTO firstLine = aqwRcvDTO.AQWRcvLineDTOs[0];

                StringBuilder sbError = new StringBuilder();

                OBAReceivementDTO erpRcvHead = new OBAReceivementDTO();

                RcvDocType rcvDocType = RcvDocType;
                if (rcvDocType != null)
                {
                    erpRcvHead.RcvDocType = new UFIDA.U9.PM.DTOs.BESimp4UIDTO();
                    erpRcvHead.RcvDocType.ID = rcvDocType.ID;
                    erpRcvHead.RcvDocType.Code = rcvDocType.Code;
                }

                erpRcvHead.BusinessDate = aqwRcvDTO.arrivetime.GetDateTime(DateTime.Today).Date;

                Supplier supt = null;

                // 当奥琦玮内料号名称前面加“冻货”两个字的，在U9内对应的供应商为“00291--北京配送部（冻货）”，没有冻货两个字的，对应的供应商为“00222-北京配送部”
                if (firstLine.lgname.Contains("冻货"))
                {
                    supt = RcvFrozenSupplier;
                }
                else
                {
                    supt = RcvSupplier;
                }

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
                //erpRcvHead.DescFlexField.PrivateDescSeg1 = aqwRcvDTO.ldiid;
                //erpRcvHead.DescFlexField.PrivateDescSeg2 = aqwRcvDTO.code;

                //if (aqwRcvDTO.sno.IsNotNullOrWhiteSpace())
                //{
                //    Warehouse wh = Warehouse.Finder.Find("Org=@Org and Code=@Code"
                //        , new OqlParam(Context.LoginOrg.ID)
                //        , new OqlParam(aqwRcvDTO.sno)
                //        );

                //    if (wh == null)
                //    {
                //        throw new BusinessException(string.Format("组织[{0}]下没有找到编码为[{1}]的收货仓库!"
                //            , Context.LoginOrg.Name
                //            , aqwRcvDTO.sno
                //            ));
                //    }
                //}

                // 现在奥琦玮内部门的编码和名称是一个字段，我让他们限定前三位是代码，后面的名称现在在调整成跟U9一致
                Department dept = null;
                Warehouse wh = null;

                string strAqwShopName = aqwRcvDTO.shopname;
                if (strAqwShopName.IsNotNullOrWhiteSpace())
                {
                    string strDept = string.Empty;
                    if (strAqwShopName.Length > 3)
                    {
                        //strDept = strAqwShopName.Substring(0, 3);
                        strDept = strAqwShopName.Substring(3, strAqwShopName.Length - 3);
                    }
                    else
                    {
                        strDept = strAqwShopName;
                    }

                    //dept = Department.Finder.Find("Org=@Org and Code=@Code"
                    dept = Department.Finder.Find("Org=@Org and Name=@Dept"
                        , new OqlParam(Context.LoginOrg.ID)
                        , new OqlParam(strDept)
                        );

                    if (dept == null)
                    {
                        throw new BusinessException(string.Format("组织[{0}]下没有找到名称为[{1}]的部门!"
                            , Context.LoginOrg.Name
                            , strDept
                            ));
                    }
                    //else
                    //{ 

                    //}

                    //string strWhOpath = string.Format("Org=@Org and (Code like '%' + @Code) order by sqlLen(Code) asc,Code asc"
                    //, strDept
                    //);
                    string strWhOpath = string.Format("Org=@Org and Department.Code = @Code order by sqlLen(Code) asc,Code asc");
                    wh = Warehouse.Finder.Find(strWhOpath
                        , new OqlParam(Context.LoginOrg.ID)
                        , new OqlParam(dept.Code)
                        );

                    if (wh == null)
                    {
                        throw new BusinessException(string.Format("组织[{0}]下没有找到所属部门为[{1}]的仓库!"
                            , Context.LoginOrg.Name
                            , dept.Code
                            ));
                    }
                }
                else
                {
                    throw new BusinessException(string.Format("单据[{0}]店铺不可为空!"
                        , aqwRcvDTO.code
                        ));
                }

                erpRcvHead.RcvLines = new List<OBARcvLineDTO>();
                foreach (AQWRcvLineDTO aqwRcvLineDTO in aqwRcvDTO.AQWRcvLineDTOs)
                {
                    if (aqwRcvLineDTO != null
                        )
                    {
                        OBARcvLineDTO erpRcvLine = GetErpRcvLine(aqwRcvLineDTO, erpRcvHead, dept, wh);

                        if (erpRcvLine != null)
                        {
                            erpRcvHead.RcvLines.Add(erpRcvLine);
                        }
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

        private static OBARcvLineDTO GetErpRcvLine(AQWRcvLineDTO aqwRcvLineDTO, OBAReceivementDTO erpRcvHead, Department dept, Warehouse wh)
        {
            OBARcvLineDTO erpRcvLine = null;
            //decimal qty = aqwRcvLineDTO.amount.GetDecimal() + aqwRcvLineDTO.damount.GetDecimal();
            decimal qty = aqwRcvLineDTO.TotalQty;

            if (qty > 0)
            {
                AQWRcvDTO aqwRcvDTO = aqwRcvLineDTO.AQWRcvHead;

                erpRcvLine = new OBARcvLineDTO();

                //erpRcvLine.ConfirmDate = aqwRcvDTO.arrivetime.GetDateTime(erpRcvHead.BusinessDate);

                erpRcvLine.ArrivedTime = aqwRcvDTO.arrivetime.GetDateTime(erpRcvHead.BusinessDate);
                erpRcvLine.ConfirmDate = erpRcvLine.ArrivedTime;

                if (aqwRcvLineDTO.lgcode.IsNotNullOrWhiteSpace())
                {
                    erpRcvLine.ItemInfo = new UFIDA.U9.CBO.SCM.Item.ItemInfo();
                    erpRcvLine.ItemInfo.ItemCode = aqwRcvLineDTO.lgcode;
                }
                else
                {
                    //sbError.AppendLine(string.Format("单据[{0}]货品[{1}]无法找到货品编码!"
                    //    , aqwRcvDTO.code
                    //    , aqwRcvLineDTO.lgid
                    //    ));
                    throw new BusinessException(string.Format("单据[{0}]货品[{1}]无法找到货品编码!"
                        , aqwRcvDTO.code
                        , aqwRcvLineDTO.lgid
                        ));
                }

                erpRcvLine.ArriveQtyTU = qty;
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

                erpRcvLine.RcvDept = new UFIDA.U9.PM.DTOs.BESimp4UIDTO();
                erpRcvLine.RcvDept.ID = dept.ID;
                erpRcvLine.RcvDept.Code = dept.Code;

                erpRcvLine.Wh = new UFIDA.U9.PM.DTOs.BESimp4UIDTO();
                erpRcvLine.Wh.ID = wh.ID;
                erpRcvLine.Wh.Code = wh.Code;

                if (wh.Manager != null)
                {
                    erpRcvLine.WhMan = new UFIDA.U9.PM.DTOs.BESimp4UIDTO();
                    erpRcvLine.WhMan.ID = wh.Manager.ID;
                    erpRcvLine.WhMan.Code = wh.Manager.Code;
                }

                if (erpRcvLine.DescFlexSegments == null)
                {
                    erpRcvLine.DescFlexSegments = new UFIDA.U9.Base.FlexField.DescFlexField.DescFlexSegments();
                }
                // 头ID
                erpRcvLine.DescFlexSegments.PrivateDescSeg9 = aqwRcvDTO.ldiid;
                // 头单号
                erpRcvLine.DescFlexSegments.PrivateDescSeg10 = aqwRcvDTO.code;
                // 行ID
                erpRcvLine.DescFlexSegments.PrivateDescSeg11 = aqwRcvLineDTO.ldiiid;

                // 档口
                erpRcvLine.DescFlexSegments.PubDescSeg12 = aqwRcvDTO.ldname;


                /*
    1、入库单价  
    2、指导价=入库单价  公共段3    (私有段的指导价不用了；)
    3、入库金额=入库单价*实到数量    私有段4
                 */
                //erpRcvLine.DescFlexSegments.PubDescSeg3 = erpRcvLine.
                // 这个写到了 头插件的  AfterValidate里了；省的计算有精度差异



                return erpRcvLine;
            }

            return erpRcvLine;
        }



        private void ApproveRcv(Receivement.EntityList lstRcv)
        {
            foreach (Receivement head in lstRcv)
            {
                //head.ActivateType = ActivateTypeEnum.UIUpdate;
                //foreach (RcvLine line in head.RcvLines)
                //{
                //    line.ActivateType = head.ActivateType;
                //}

                // 清空缓存
                UFSoft.UBF.PL.Engine.Cache.PLCacheManager.DataCache.FlushCache();
                UFSoft.UBF.PL.Engine.Cache.PLCacheManager.ObjectCache.FlushCache();

                {
                    // 改为建华给的BP
                    UFIDA.U9.PM.Rcv.Proxy.RcvApprovedBPProxy proxy = new UFIDA.U9.PM.Rcv.Proxy.RcvApprovedBPProxy();
                    proxy.DocHead = head.ID;
                    // 提交7，审核8，弃审9
                    proxy.ActType = 7;
                    proxy.Do();
                }

                // 清空缓存，否则报并发
                UFSoft.UBF.PL.Engine.Cache.PLCacheManager.DataCache.FlushCache();
                UFSoft.UBF.PL.Engine.Cache.PLCacheManager.ObjectCache.FlushCache();
                {
                    // 改为建华给的BP
                    UFIDA.U9.PM.Rcv.Proxy.RcvApprovedBPProxy proxy = new UFIDA.U9.PM.Rcv.Proxy.RcvApprovedBPProxy();
                    proxy.DocHead = head.ID;
                    // 提交7，审核8，弃审9
                    proxy.ActType = 8;
                    proxy.Do();
                }
            }
        }



        private string PrepareData(List<AQWRcvDTO> lstRcvHead)
        {
            StringBuilder sbError = new StringBuilder();

            if (lstRcvHead != null
                && lstRcvHead.Count > 0
                )
            {
                foreach (AQWRcvDTO headDTO in lstRcvHead)
                {
                    // 现在奥琦玮内部门的编码和名称是一个字段，我让他们限定前三位是代码，后面的名称现在在调整成跟U9一致
                    Department dept = null;
                    Warehouse wh = null;

                    string strAqwShopName = headDTO.shopname;
                    if (strAqwShopName.IsNotNullOrWhiteSpace())
                    {
                        string strDept = string.Empty;
                        if (strAqwShopName.Length > 3)
                        {
                            //strDept = strAqwShopName.Substring(0, 3);
                            strDept = strAqwShopName.Substring(3, strAqwShopName.Length - 3);
                        }
                        else
                        {
                            strDept = strAqwShopName;
                        }

                        //dept = Department.Finder.Find("Org=@Org and Code=@Code"
                        dept = Department.Finder.Find("Org=@Org and Name=@Dept"
                            , new OqlParam(Context.LoginOrg.ID)
                            , new OqlParam(strDept)
                            );

                        if (dept == null)
                        {
                            sbError.AppendLine(string.Format("组织[{0}]下没有找到名称为[{1}]的部门!"
                                , Context.LoginOrg.Name
                                , strDept
                                ));

                            continue;
                        }
                        else
                        {
                            headDTO.DepartmentID = dept.ID;
                            headDTO.DepartmentCode = dept.Code;
                            headDTO.DepartmentName = dept.Name;
                        }
                        //else
                        //{ 

                        //}

                        //string strWhOpath = string.Format("Org=@Org and (Code like '%' + @Code) order by sqlLen(Code) asc,Code asc"
                        //, strDept
                        //);
                        string strWhOpath = string.Format("Org=@Org and Department.Code = @Code order by sqlLen(Code) asc,Code asc");
                        wh = Warehouse.Finder.Find(strWhOpath
                            , new OqlParam(Context.LoginOrg.ID)
                            , new OqlParam(dept.Code)
                            );

                        if (wh == null)
                        {
                            sbError.AppendLine(string.Format("组织[{0}]下没有找到所属部门为[{1}]的仓库!"
                                , Context.LoginOrg.Name
                                , dept.Code
                                ));

                            continue;
                        }
                        else
                        {
                            headDTO.WarehouseID = wh.ID;
                            headDTO.WarehouseCode = wh.Code;
                            headDTO.WarehouseName = wh.Name;
                        }
                    }
                    else
                    {
                        sbError.AppendLine(string.Format("单据[{0}]店铺不可为空!"
                            , headDTO.code
                            ));

                        continue;
                    }


                    foreach (AQWRcvLineDTO lineDTO in headDTO.AQWRcvLineDTOs)
                    {
                        string strItemCode = lineDTO.lgcode;


                        //deptLine = U9.VOB.Cus.HBHJianLiYuan.DeptItemSupplierBE.DeptItemSupplierLine.Finder.Find("ItemMaster.Code='" + line.ItemInfo.ItemID.Code + "' and DeptItemSupplier.Department.Name='" + line.ReqDept.Name + "'");
                        U9.VOB.Cus.HBHJianLiYuan.DeptItemSupplierBE.DeptItemSupplierLine deptLine = U9.VOB.Cus.HBHJianLiYuan.DeptItemSupplierBE.DeptItemSupplierLine.Finder.Find("ItemMaster.Code=@ItemCode and DeptItemSupplier.Department.Name=@DeptName order by Supplier desc ");
                        if (deptLine != null
                            && deptLine.Supplier != null
                            )
                        {
                        }

                    }
                }
            }

            return sbError.ToString();
        }
    }


    // 供应商类型
    /// <summary>
    /// 供应商类型
    /// </summary>
    public enum AQWSupplierType
    { 
        Empty = -1 ,
        冷冻 = 0 ,
        配送 = 1 ,
    }
}