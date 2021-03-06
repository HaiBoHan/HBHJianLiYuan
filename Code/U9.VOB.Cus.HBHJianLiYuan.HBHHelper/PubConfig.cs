﻿using System;
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
        // 第二部分需求
        /// <summary>
        /// 第二部分需求
        /// </summary>
        public static bool Const_TwoStage = true;

        // 第三阶段需求(HR)
        /// <summary>
        /// 第三阶段需求(HR)
        /// </summary>
        public static bool Const_ThirdHRStage = true;

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

                    //_dicConfig.Add("WebPartExtend_HBH_JianLiYuan.config", "WebPartExtend_HBH_JianLiYuan.config");
                    //_dicConfig.Add("U9.VOB.Cus.HBHJianLiYuan.PlugInBE.sub.xml", "bin\\U9.VOB.Cus.HBHJianLiYuan.PlugInBE.sub.xml");
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
        //public const long expire = 130855104000000000;
        //public const long expire = 131065056000000000;   //2016.05.01
        //public const long expire = 130933728000000000;   //2015.12.01
        //public const long expire = 130960512000000000;   //2016.01.01
        //public const long expire = 131012352000000000;   //2016.03.01
        //public const long expire = 131039136000000000;   //2016.04.01
        //public const long expire = 131065056000000000;   //2016.05.01
        //public const long expire = 131117760000000000;   //2016.07.01
        //public const long expire = 131144544000000000;   //2016.08.01
        //public const long expire = 131171328000000000;   //2016.09.01
        //public const long expire = 131224032000000000;   //2016.11.01
        //public const long expire = 131276736000000000;   //2017.01.01
        //public const long expire = 131327712000000000;   //2017.03.01
        //public const long expire = 131407200000000000;   //2017.06.01
        //public const long expire = 131565312000000000;   //2017.12.01

        public static void ExpiredProcess()
        {
            long strNow = DateTime.Now.ToFileTime();

            //if (strNow > expire)
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
