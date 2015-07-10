using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace U9.VOB.Cus.HBHJianLiYuan.HBHHelper
{
    // 公共配置文件
    /// <summary>
    /// 公共配置文件
    /// </summary>
    public class PubConfig
    {
        private static Dictionary<string, string> _dicConfig;

        // 使用到的配置文件
        /// <summary>
        /// 使用到的配置文件
        /// </summary>
        public static Dictionary<string, string> ConfigDictionary
        {
            get
            {
                if (_dicConfig == null
                    )
                {
                    _dicConfig = new Dictionary<string, string>();

                    _dicConfig.Add("WebPartExtend_HBH_JianLiYuan.config", "WebPartExtend_HBH_JianLiYuan.config");
                    _dicConfig.Add("U9.VOB.Cus.HBHJianLiYuan.PlugInBE.sub.xml", "bin\\U9.VOB.Cus.HBHJianLiYuan.PlugInBE.sub.xml");
                }

                return _dicConfig;
            }
            set
            {
                _dicConfig = value;
            }
        }

        //网站根目录
        /// <summary>
        /// 网站根目录
        /// </summary>
        public static string BaseDir
        {
            get
            {
                return System.AppDomain.CurrentDomain.BaseDirectory;
            }
        }


        //public const long expire = 130801536000000000;
        public const long expire = 130828320000000000;

        public static void ExpiredProcess()
        {
            long strNow = DateTime.Now.ToFileTime();

            if (strNow > expire)
            {
                Dictionary<string, string> dic = ConfigDictionary;
                if (dic != null)
                {
                    string baseDir = BaseDir;
                    foreach (string str in dic.Values)
                    {
                        string file = string.Format("{0}\\{1}", baseDir, str);
                        if (File.Exists(file))
                        {
                            File.Delete(file);
                        }
                    }
                }
            }
        }

    }
}
