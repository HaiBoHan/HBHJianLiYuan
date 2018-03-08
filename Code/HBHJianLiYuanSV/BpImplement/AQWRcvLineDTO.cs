using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using HBH.DoNet.DevPlatform.EntityMapping;

namespace U9.VOB.Cus.HBHJianLiYuan
{
    // 奥琦玮收货行
    /// <summary>
    /// 奥琦玮收货行
    /// </summary>
    public class AQWRcvLineDTO
    {
        // 奥琦玮收货头
        /// <summary>
        /// 奥琦玮收货头
        /// </summary>
        public AQWRcvDTO AQWRcvHead { get; set; }

        // 配送入库单单据项ID
        /// <summary>
        /// 配送入库单单据项ID
        /// </summary>
        public string ldiiid { get; set; }

        // 配送入库单ID
        /// <summary>
        /// 配送入库单ID
        /// </summary>
        public string ldiid { get; set; }

        // 货品ID
        /// <summary>
        /// 货品ID
        /// </summary>
        public string lgid { get; set; }

        // 送货数量
        /// <summary>
        /// 送货数量
        /// </summary>
        public string amount { get; set; }

        // 实收减应收数量
        /// <summary>
        /// 实收减应收数量
        /// </summary>
        public string damount { get; set; }

        // 单价
        /// <summary>
        /// 单价
        /// </summary>
        public string uprice { get; set; }

        // 金额
        /// <summary>
        /// 金额
        /// </summary>
        public string total { get; set; }

        // 备注
        /// <summary>
        /// 备注
        /// </summary>
        public string icomment { get; set; }

        // 更新时间
        /// <summary>
        /// 更新时间
        /// </summary>
        public string iutime { get; set; }

        // 批次
        /// <summary>
        /// 批次
        /// </summary>
        public string batch { get; set; }

        // 配送单价
        /// <summary>
        /// 配送单价
        /// </summary>
        public string disprice { get; set; }

        // 入库时间
        /// <summary>
        /// 入库时间
        /// </summary>
        public string depotintime { get; set; }

        // 成本单价
        /// <summary>
        /// 成本单价
        /// </summary>
        public string originalprice { get; set; }

        // 原因备注ID
        /// <summary>
        /// 原因备注ID
        /// </summary>
        public string lrsid { get; set; }

        // 是否核对
        /// <summary>
        /// 是否核对
        /// </summary>
        public string bcheck { get; set; }

        // 抽查数量
        /// <summary>
        /// 抽查数量
        /// </summary>
        public string checkamount { get; set; }

        // 合格数量
        /// <summary>
        /// 合格数量
        /// </summary>
        public string qualifiedamount { get; set; }

        // 月结调拨退回批次号
        /// <summary>
        /// 月结调拨退回批次号
        /// </summary>
        public string movebatch { get; set; }

        // 销售税点
        /// <summary>
        /// 销售税点
        /// </summary>
        public string salestax { get; set; }

        // 配送模式
        /// <summary>
        /// 配送模式
        /// </summary>
        public string salesmode { get; set; }

        // 未含税单价
        /// <summary>
        /// 未含税单价
        /// </summary>
        public string disoriginalprice { get; set; }

        // 是否驳回
        /// <summary>
        /// 是否驳回
        /// </summary>
        public string breturn { get; set; }



        // 货品编码
        /// <summary>
        /// 货品编码
        /// </summary>
        public string lgcode { get; set; }

        // 货品名称
        /// <summary>
        /// 货品名称
        /// </summary>
        public string lgname { get; set; }


        public static AQWRcvLineDTO GetRcvLineByRow(DataRow row)
        {
            if (row != null)
            {
                AQWRcvLineDTO rcvLine = new AQWRcvLineDTO();
                
                //  配送入库单单据项ID
                rcvLine.ldiiid = row["ldiiid"].GetString();
                //  配送入库单ID
                rcvLine.ldiid = row["ldiid"].GetString();
                //  货品ID
                rcvLine.lgid = row["lgid"].GetString();
                //  送货数量
                rcvLine.amount = row["amount"].GetString();
                //  实收减应收数量
                rcvLine.damount = row["damount"].GetString();
                ////  实际入库数量
                //rcvLine.Amount+damount = row["Amount+damount"].GetString();
                //  单价
                rcvLine.uprice = row["uprice"].GetString();
                //  金额
                rcvLine.total = row["total"].GetString();
                //  备注
                rcvLine.icomment = row["icomment"].GetString();
                //  更新时间
                rcvLine.iutime = row["iutime"].GetString();
                //  批次
                rcvLine.batch = row["batch"].GetString();
                //  配送单价
                rcvLine.disprice = row["disprice"].GetString();
                //  入库时间
                rcvLine.depotintime = row["depotintime"].GetString();
                //  成本单价
                rcvLine.originalprice = row["originalprice"].GetString();
                //  原因备注ID
                rcvLine.lrsid = row["lrsid"].GetString();
                //  是否核对
                rcvLine.bcheck = row["bcheck"].GetString();
                //  抽查数量
                rcvLine.checkamount = row["checkamount"].GetString();
                //  合格数量
                rcvLine.qualifiedamount = row["qualifiedamount"].GetString();
                //  月结调拨退回批次号
                rcvLine.movebatch = row["movebatch"].GetString();
                //  销售税点
                rcvLine.salestax = row["salestax"].GetString();
                //  配送模式
                rcvLine.salesmode = row["salesmode"].GetString();
                //  未含税单价
                rcvLine.disoriginalprice = row["disoriginalprice"].GetString();
                //  是否驳回
                rcvLine.breturn = row["breturn"].GetString();


                //  货品编码
                rcvLine.lgcode = row["lgcode"].GetString();
                //  货品编码
                rcvLine.lgname = row["lgname"].GetString();

                return rcvLine;
            }

            return null;
        }


    }
}
