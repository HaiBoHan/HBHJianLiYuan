using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace U9.VOB.Cus.HBHJianLiYuan
{
    public class AQWRcvLineDTO
    {
    
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


    }
}
