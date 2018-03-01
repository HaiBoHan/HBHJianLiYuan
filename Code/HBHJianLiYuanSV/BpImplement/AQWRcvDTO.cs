using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using HBH.DoNet.DevPlatform.EntityMapping;

namespace U9.VOB.Cus.HBHJianLiYuan
{
    // 奥琦玮收货头
    /// <summary>
    /// 奥琦玮收货头
    /// </summary>
    public class AQWRcvDTO
    {

        // 配送入库单ID
        /// <summary>
        /// 配送入库单ID
        /// </summary>
        public string ldiid { get; set; }

        // 单号
        /// <summary>
        /// 单号
        /// </summary>
        public string code { get; set; }

        // 配送中心ID
        /// <summary>
        /// 配送中心ID
        /// </summary>
        public string lsid { get; set; }

        // 所属仓库ID
        /// <summary>
        /// 所属仓库ID
        /// </summary>
        public string ldid { get; set; }

        // 单据状态
        /// <summary>
        /// 单据状态
        /// </summary>
        public string status { get; set; }

        // 配送出库单ID
        /// <summary>
        /// 配送出库单ID
        /// </summary>
        public string ldoid { get; set; }

        // 创建人ID
        /// <summary>
        /// 创建人ID
        /// </summary>
        public string cuid { get; set; }

        // 创建时间
        /// <summary>
        /// 创建时间
        /// </summary>
        public string ctime { get; set; }

        // 审核人ID
        /// <summary>
        /// 审核人ID
        /// </summary>
        public string auid { get; set; }

        // 审核日期
        /// <summary>
        /// 审核日期
        /// </summary>
        public string atime { get; set; }

        // 经办人ID
        /// <summary>
        /// 经办人ID
        /// </summary>
        public string ouid { get; set; }

        // 备注
        /// <summary>
        /// 备注
        /// </summary>
        public string comment { get; set; }

        // 修改时间
        /// <summary>
        /// 修改时间
        /// </summary>
        public string utime { get; set; }

        // 是否结算
        /// <summary>
        /// 是否结算
        /// </summary>
        public string bsetcheck { get; set; }

        // 原因备注ID
        /// <summary>
        /// 原因备注ID
        /// </summary>
        public string lrsid { get; set; }

        // 存储用友单号
        /// <summary>
        /// 存储用友单号
        /// </summary>
        public string nccode { get; set; }

        // 到货时间
        /// <summary>
        /// 到货时间
        /// </summary>
        public string arrivetime { get; set; }

        // 是否稽查
        /// <summary>
        /// 是否稽查
        /// </summary>
        public string bcheck { get; set; }

        // 稽查人
        /// <summary>
        /// 稽查人
        /// </summary>
        public string buid { get; set; }

        // 稽查时间
        /// <summary>
        /// 稽查时间
        /// </summary>
        public string btime { get; set; }

        // 盘点周期tag
        /// <summary>
        /// 盘点周期tag
        /// </summary>
        public string pdatetag { get; set; }

        // 单据类型
        /// <summary>
        /// 单据类型
        /// </summary>
        public string ordertype { get; set; }

        // 确认人
        /// <summary>
        /// 确认人
        /// </summary>
        public string confirmuid { get; set; }

        // 确认时间
        /// <summary>
        /// 确认时间
        /// </summary>
        public string confirmtime { get; set; }

        // 驳回理由Id
        /// <summary>
        /// 驳回理由Id
        /// </summary>
        public string rlrsid { get; set; }

        // 驳回人ID
        /// <summary>
        /// 驳回人ID
        /// </summary>
        public string returnuid { get; set; }

        // 驳回时间
        /// <summary>
        /// 驳回时间
        /// </summary>
        public string returntime { get; set; }

        // 是否已生成调拨领用单
        /// <summary>
        /// 是否已生成调拨领用单
        /// </summary>
        public string bgenerateorder { get; set; }


        // 仓库编码
        /// <summary>
        /// 仓库编码
        /// </summary>
        public string sno { get; set; }

        // 仓库名称
        /// <summary>
        /// 仓库名称
        /// </summary>
        public string ldname { get; set; }

        // 店铺编码
        /// <summary>
        /// 店铺编码
        /// </summary>
        public string shopcode { get; set; }

        // 店铺名称
        /// <summary>
        /// 店铺名称
        /// </summary>
        public string shopname { get; set; }


        // 行
        /// <summary>
        /// 行
        /// </summary>
        public List<AQWRcvLineDTO> AQWRcvLineDTOs { get; set; }



        public static List<AQWRcvDTO> GetAQWRcvByDataset(DataSet ds)
        {
            List<AQWRcvDTO> lstRcv = new List<AQWRcvDTO>();

            if (ds != null
                && ds.Tables != null
                && ds.Tables.Count >= 1
                )
            {
                Dictionary<string, AQWRcvDTO> dicRcvHead = new Dictionary<string, AQWRcvDTO>();

                {
                    DataTable dt1 = ds.Tables[0];

                    if (dt1.Rows != null
                        && dt1.Rows.Count > 0
                        )
                    {
                        foreach (DataRow row in dt1.Rows)
                        {
                            AQWRcvDTO rcvHead = GetRcvLineByRow(row);

                            string id = rcvHead.ldiid;

                            if (!dicRcvHead.ContainsKey(id))
                            {
                                dicRcvHead.Add(id, rcvHead);

                                lstRcv.Add(rcvHead);
                            }
                        }
                    }
                }

                if (ds.Tables.Count >= 2)
                {
                    DataTable dt2 = ds.Tables[1];

                    if (dt2.Rows != null
                        && dt2.Rows.Count > 0
                        )
                    {
                        foreach (DataRow row in dt2.Rows)
                        {
                            AQWRcvLineDTO rcvLine = AQWRcvLineDTO.GetRcvLineByRow(row);

                            string headID = rcvLine.ldiid;

                            if (dicRcvHead.ContainsKey(headID))
                            {
                                AQWRcvDTO rcvHead = dicRcvHead[headID];

                                if (rcvHead.AQWRcvLineDTOs == null)
                                {
                                    rcvHead.AQWRcvLineDTOs = new List<AQWRcvLineDTO>();
                                }

                                rcvHead.AQWRcvLineDTOs.Add(rcvLine);
                            }

                        }
                    }

                }
            }

            return lstRcv;
        }


        public static AQWRcvDTO GetRcvLineByRow(DataRow row)
        {
            if (row != null)
            {
                AQWRcvDTO rcvHead = new AQWRcvDTO();
                                
                //  配送入库单ID
                rcvHead.ldiid = row["ldiid"].GetString();
                //  单号
                rcvHead.code = row["code"].GetString();
                //  配送中心ID
                rcvHead.lsid = row["lsid"].GetString();
                //  所属仓库ID
                rcvHead.ldid = row["ldid"].GetString();
                //  单据状态
                rcvHead.status = row["status"].GetString();
                //  配送出库单ID
                rcvHead.ldoid = row["ldoid"].GetString();
                //  创建人ID
                rcvHead.cuid = row["cuid"].GetString();
                //  创建时间
                rcvHead.ctime = row["ctime"].GetString();
                //  审核人ID
                rcvHead.auid = row["auid"].GetString();
                //  审核日期
                rcvHead.atime = row["atime"].GetString();
                //  经办人ID
                rcvHead.ouid = row["ouid"].GetString();
                //  备注
                rcvHead.comment = row["comment"].GetString();
                //  修改时间
                rcvHead.utime = row["utime"].GetString();
                //  是否结算
                rcvHead.bsetcheck = row["bsetcheck"].GetString();
                //  原因备注ID
                rcvHead.lrsid = row["lrsid"].GetString();
                //  存储用友单号
                rcvHead.nccode = row["nccode"].GetString();
                //  到货时间
                rcvHead.arrivetime = row["arrivetime"].GetString();
                //  是否稽查
                rcvHead.bcheck = row["bcheck"].GetString();
                //  稽查人
                rcvHead.buid = row["buid"].GetString();
                //  稽查时间
                rcvHead.btime = row["btime"].GetString();
                //  盘点周期tag
                rcvHead.pdatetag = row["pdatetag"].GetString();
                //  单据类型
                rcvHead.ordertype = row["ordertype"].GetString();

                //  确认人
                rcvHead.confirmuid = row["confirmuid"].GetString();
                //  确认时间
                rcvHead.confirmtime = row["confirmtime"].GetString();
                //  驳回理由Id
                rcvHead.rlrsid = row["rlrsid"].GetString();
                //  驳回人ID
                rcvHead.returnuid = row["returnuid"].GetString();
                //  驳回时间
                rcvHead.returntime = row["returntime"].GetString();
                //  是否已生成调拨领用单
                rcvHead.bgenerateorder = row["bgenerateorder"].GetString();

                //  仓库编码
                rcvHead.sno = row["sno"].GetString();
                //  仓库名称
                rcvHead.ldname = row["ldname"].GetString();
                //  部门编码
                rcvHead.shopcode = row["shopcode"].GetString();
                //  部门名称
                rcvHead.shopname = row["shopname"].GetString();

                return rcvHead;
            }

            return null;
        }


    }
}



