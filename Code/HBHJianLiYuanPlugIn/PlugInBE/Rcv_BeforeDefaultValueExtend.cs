using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UFSoft.UBF.Business;
using UFIDA.U9.SM.SO;
using UFSoft.UBF.PL;
using UFIDA.U9.SM.Enums;
using UFIDA.U9.PM.Rcv;

namespace U9.VOB.Cus.HBHJianLiYuan.PlugInBE
{
    public class Rcv_BeforeDefaultValueExtend : UFSoft.UBF.Eventing.IEventSubscriber
    {
        public void Notify(params object[] args)
        {
            if (args == null || args.Length == 0 || !(args[0] is UFSoft.UBF.Business.EntityEvent))
                return;
            BusinessEntity.EntityKey key = ((UFSoft.UBF.Business.EntityEvent)args[0]).EntityKey;

            if (key == null)
                return;
            UFIDA.U9.PM.Rcv.Receivement rcv = key.GetEntity() as UFIDA.U9.PM.Rcv.Receivement;//收货单实体
            if (rcv == null)
                return;
            bool isSubmit = false;
            if (rcv.OriginalData.Status == RcvStatusEnum.Opened && rcv.Status == RcvStatusEnum.Approving)
            {
                isSubmit = true;
            }
            foreach (RcvLine line in rcv.RcvLines)
            {
                //取采购订单折扣率、折扣额
                if (rcv.SrcDocType == UFIDA.U9.PM.Enums.RcvSrcDocTypeEnum.PO)//来源采购订单，可取到折扣率、折扣额
                {
                    UFIDA.U9.PM.PO.POLine poline = UFIDA.U9.PM.PO.POLine.Finder.FindByID(line.SrcPO.SrcDocLine.EntityID);
                    if (poline != null)
                    {
                        line.DescFlexSegments.PubDescSeg1 = poline.DescFlexSegments.PubDescSeg1;
                        line.DescFlexSegments.PubDescSeg2 = poline.DescFlexSegments.PubDescSeg2;
                    }
                    //修改折前价格（扩展字段1），计算最终价
                    if (line.DescFlexSegments != null && line.DescFlexSegments.PrivateDescSeg1.ToString() != "")//折前价格不为空
                    {
                        if (line.DescFlexSegments.PubDescSeg1.ToString() != "")//有折扣率，先按照折扣率算最终价
                        {
                            line.FinallyPriceTC = Math.Round(Decimal.Parse(line.DescFlexSegments.PubDescSeg1) * Decimal.Parse(line.DescFlexSegments.PrivateDescSeg1), line.BalanceCurrency.PriceRound.Precision);
                        }
                        else if (line.DescFlexSegments.PubDescSeg2.ToString() != "")
                        {
                            line.FinallyPriceTC = Decimal.Parse(line.DescFlexSegments.PrivateDescSeg1) - Decimal.Parse(line.DescFlexSegments.PubDescSeg2);
                        }
                        else
                        {
                            line.FinallyPriceTC = Decimal.Parse(line.DescFlexSegments.PrivateDescSeg1);
                        }
                        #region 重算价格
                        //价格重算

                        if (rcv.IsPriceIncludeTax)
                        {
                            line.ArriveTotalMnyTC = rcv.AC.MoneyRound.GetRoundValue(line.FinallyPriceTC * line.ArriveQtyPU);//  价税合计(原币)  

                            if (line.TaxRate == 0)
                            {
                                line.ArriveTotalTaxTC = 0;//税额(原本)
                                line.ArriveTotalNetMnyTC = rcv.AC.MoneyRound.GetRoundValue(line.ArriveTotalMnyTC - line.ArriveTotalTaxTC);//  未税金额(原币) 
                            }
                            else if (line.TaxSchedule.TaxScheduleTaxs[0].Tax.TaxAmountCalMethod == UFIDA.U9.CBO.FI.Tax.TaxAmountCalMethodEnum.TaxByMoneyEmbedded)
                            {
                                line.ArriveTotalTaxTC = rcv.AC.MoneyRound.GetRoundValue(line.ArriveTotalMnyTC * line.TaxRate);//税额(原本)
                                line.ArriveTotalNetMnyTC = rcv.AC.MoneyRound.GetRoundValue(line.ArriveTotalMnyTC - line.ArriveTotalTaxTC);//  未税金额(原币)                     
                            }
                            else if (line.TaxSchedule.TaxScheduleTaxs[0].Tax.TaxAmountCalMethod == UFIDA.U9.CBO.FI.Tax.TaxAmountCalMethodEnum.TaxByMoneyAdded)
                            {
                                line.ArriveTotalNetMnyTC = rcv.AC.MoneyRound.GetRoundValue(line.ArriveTotalMnyTC / (1 + line.TaxRate));//  未税金额(原币)  
                                line.ArriveTotalTaxTC = rcv.AC.MoneyRound.GetRoundValue(line.ArriveTotalMnyTC - line.ArriveTotalNetMnyTC);//税额(原本) 
                            }
                        }
                        else
                        {
                            line.ArriveTotalNetMnyTC = rcv.AC.MoneyRound.GetRoundValue(line.FinallyPriceTC * line.ArriveQtyPU);//未税金额(原币) 
                            if (line.TaxRate == 0)
                            {
                                line.ArriveTotalMnyTC = rcv.AC.MoneyRound.GetRoundValue(line.ArriveTotalNetMnyTC);//价税合计(原币)  
                                line.ArriveTotalTaxTC = 0;//税额(原本)
                            }
                            else if (line.TaxSchedule.TaxScheduleTaxs[0].Tax.TaxAmountCalMethod == UFIDA.U9.CBO.FI.Tax.TaxAmountCalMethodEnum.TaxByMoneyEmbedded)
                            {
                                line.ArriveTotalMnyTC = rcv.AC.MoneyRound.GetRoundValue(line.ArriveTotalNetMnyTC / (1 - line.TaxRate));//价税合计(原币)  
                                line.ArriveTotalTaxTC = rcv.AC.MoneyRound.GetRoundValue(line.ArriveTotalMnyTC - line.ArriveTotalNetMnyTC);//税额(原本)
                            }
                            else if (line.TaxSchedule.TaxScheduleTaxs[0].Tax.TaxAmountCalMethod == UFIDA.U9.CBO.FI.Tax.TaxAmountCalMethodEnum.TaxByMoneyAdded)
                            {
                                line.ArriveTotalTaxTC = rcv.AC.MoneyRound.GetRoundValue(line.ArriveTotalNetMnyTC * line.TaxRate);//税额(原本)
                                line.ArriveTotalMnyTC = rcv.AC.MoneyRound.GetRoundValue(line.ArriveTotalNetMnyTC + line.ArriveTotalTaxTC);//价税合计(原币)                        
                            }
                        }
                        line.TotalMnyTC = line.ArriveTotalMnyTC;
                        line.TotalTaxTC = line.ArriveTotalTaxTC;
                        line.TotalNetMnyTC = line.ArriveTotalNetMnyTC;
                        #endregion
                }
               
                Session.Current.InList(line);
                }
                //提交，单价写入批号主档
                if (isSubmit && line.InvLot != null)
                {
                    using (ISession session = Session.Open())
                    {
                        UFIDA.U9.Lot.LotMaster lotMaster = UFIDA.U9.Lot.LotMaster.Finder.FindByID(line.InvLot.ID);
                        lotMaster.DescFlexSegments.PrivateDescSeg1 = line.FinallyPriceTC.ToString();
                        session.Commit();
                    }
                }
            }
        }

    }
}
