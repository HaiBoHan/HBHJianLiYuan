using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UFSoft.UBF.Business;
using UFIDA.U9.SM.SO;
using UFSoft.UBF.PL;
using UFIDA.U9.SM.Enums;
using UFIDA.U9.PR.PurchaseRequest;
using U9.VOB.Cus.HBHJianLiYuan.HBHHelper;
using HBH.DoNet.DevPlatform.EntityMapping;
using UFIDA.U9.CBO.SCM.Supplier;
using UFIDA.U9.Base;
using UFIDA.U9.CBO.HR.Department;
using UFIDA.U9.InvDoc.TransferIn;
using UFIDA.U9.Lot;

namespace U9.VOB.Cus.HBHJianLiYuan.PlugInBE
{
    public class TransferIn_AfterUpdated : UFSoft.UBF.Eventing.IEventSubscriber
    {
        public void Notify(params object[] args)
        {
            if (args == null || args.Length == 0 || !(args[0] is UFSoft.UBF.Business.EntityEvent))
                return;
            BusinessEntity.EntityKey key = ((UFSoft.UBF.Business.EntityEvent)args[0]).EntityKey;

            if (key == null)
                return;
            TransferIn entity = key.GetEntity() as TransferIn;//请购单实体
            if (entity == null)
                return;

            // 审核时，写入  发放时间(最后发放时间)、已发放时间(累计所有发放时间)、发放次数
            // 调入单行，发放时间  =  私有段2
            // 批号，发放时间 = 私有段4 ； 已用时间 = 私有单5 ； 使用次数 = 私有段6 ；
            if (entity.Status == TransInStatus.Approved
                && entity.OriginalData.Status == TransInStatus.Approving
                )
            {
                using (ISession session = Session.Open())
                {
                    bool isUpdated = false;

                    foreach (var line in entity.TransInLines)
                    {
                        if (line != null
                            && line.LotInfo != null
                            && line.LotInfo.LotMaster != null
                            )
                        {
                            LotMaster lotMaster = line.LotInfo.LotMaster;
                            string strGetDate = line.DescFlexSegments.PrivateDescSeg2;

                            if (strGetDate.IsNotNullOrWhiteSpace()
                                && lotMaster.DescFlexSegments.PrivateDescSeg4 != strGetDate
                                )
                            {
                                lotMaster.DescFlexSegments.PrivateDescSeg4 = strGetDate;
                                //if (lotMaster.DescFlexSegments.PrivateDescSeg5.IsNotNullOrWhiteSpace())
                                //{
                                //    lotMaster.DescFlexSegments.PrivateDescSeg5 += ";" + strGetDate;
                                //}
                                //else
                                //{
                                //    lotMaster.DescFlexSegments.PrivateDescSeg5 = strGetDate;
                                //}
                                lotMaster.DescFlexSegments.PrivateDescSeg5 += strGetDate + ";";
                                int number = lotMaster.DescFlexSegments.PrivateDescSeg6.GetInt();
                                number++;
                                lotMaster.DescFlexSegments.PrivateDescSeg6 = number.ToString();

                                isUpdated = true;
                            }
                        }
                    }

                    if (isUpdated)
                    {
                        session.Commit();
                    }
                }
            }
            else if (entity.OriginalData.Status == TransInStatus.Approved
                && (entity.Status == TransInStatus.Approving
                    || entity.Status == TransInStatus.Opening
                    )
                )
            {
                using (ISession session = Session.Open())
                {
                    bool isUpdated = false;

                    foreach (var line in entity.TransInLines)
                    {
                        if (line != null
                            && line.LotInfo != null
                            && line.LotInfo.LotMaster != null
                            )
                        {
                            LotMaster lotMaster = line.LotInfo.LotMaster;
                            string strGetDate = line.DescFlexSegments.PrivateDescSeg2;

                            if (strGetDate.IsNotNullOrWhiteSpace()
                                )
                            {
                                if (lotMaster.DescFlexSegments.PrivateDescSeg4 == strGetDate)
                                {
                                    string strGetDateEnd = strGetDate + ";";
                                    if (lotMaster.DescFlexSegments.PrivateDescSeg5.EndsWith(strGetDateEnd))
                                    {
                                        lotMaster.DescFlexSegments.PrivateDescSeg5 = lotMaster.DescFlexSegments.PrivateDescSeg5.Remove(strGetDateEnd.Length - 1, strGetDateEnd.Length);

                                        if (lotMaster.DescFlexSegments.PrivateDescSeg5.Length > 0)
                                        {
                                            string[] splitDate = lotMaster.DescFlexSegments.PrivateDescSeg5.Split(';');

                                            if (splitDate.Length > 0)
                                            {
                                                string strOldDate = splitDate[splitDate.Length - 1];
                                                lotMaster.DescFlexSegments.PrivateDescSeg4 = strOldDate;
                                            }
                                        }

                                        int number = lotMaster.DescFlexSegments.PrivateDescSeg6.GetInt();
                                        if (number > 0)
                                        {
                                            number--;
                                            lotMaster.DescFlexSegments.PrivateDescSeg6 = number.ToString();
                                        }
                                        isUpdated = true;
                                    }
                                }
                                //if (lotMaster.DescFlexSegments.PrivateDescSeg4 == strGetDate)
                                else
                                {
                                    string strGetDateEnd = strGetDate + ";";
                                    if (lotMaster.DescFlexSegments.PrivateDescSeg5.EndsWith(strGetDateEnd))
                                    {
                                        lotMaster.DescFlexSegments.PrivateDescSeg5 = lotMaster.DescFlexSegments.PrivateDescSeg5.Remove(strGetDateEnd.Length - 1, strGetDateEnd.Length);

                                        //if (lotMaster.DescFlexSegments.PrivateDescSeg5.Length > 0)
                                        //{
                                        //    string[] splitDate = lotMaster.DescFlexSegments.PrivateDescSeg5.Split(';');

                                        //    if (splitDate.Length > 0)
                                        //    {
                                        //        string strOldDate = splitDate[splitDate.Length - 1];
                                        //        lotMaster.DescFlexSegments.PrivateDescSeg4 = strOldDate;
                                        //    }
                                        //}

                                        int number = lotMaster.DescFlexSegments.PrivateDescSeg6.GetInt();
                                        if (number > 0)
                                        {
                                            number--;
                                            lotMaster.DescFlexSegments.PrivateDescSeg6 = number.ToString();
                                        }
                                        isUpdated = true;
                                    }
                                    else if (lotMaster.DescFlexSegments.PrivateDescSeg5.Contains(strGetDateEnd))
                                    {
                                        int lastIndex = lotMaster.DescFlexSegments.PrivateDescSeg5.LastIndexOf(strGetDateEnd);
                                        if (lastIndex >= 0)
                                        {
                                            lotMaster.DescFlexSegments.PrivateDescSeg5 = lotMaster.DescFlexSegments.PrivateDescSeg5.Remove(lastIndex, strGetDateEnd.Length);

                                            int number = lotMaster.DescFlexSegments.PrivateDescSeg6.GetInt();
                                            if (number > 0)
                                            {
                                                number--;
                                                lotMaster.DescFlexSegments.PrivateDescSeg6 = number.ToString();
                                            }
                                            isUpdated = true;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (isUpdated)
                    {
                        session.Commit();
                    }
                }
            }
        }
        
    }
}
