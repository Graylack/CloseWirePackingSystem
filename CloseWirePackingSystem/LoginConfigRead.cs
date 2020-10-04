using System;
using System.Xml.Linq;
using System.Text;
using System.Windows;

namespace CloseWirePackingSystem
{
    class LoginConfigRead : XmlBase
    {
        public LoginConfigRead() { 
            PermissionSet = GetPermission();
        }

        #region 属性以及全局变量
        public string ConnectionString { get; private set; }
        public string ConnectionDriver { get; private set; }
        public string LogoPath { get; private set; }
        public string BackGroundStartPointColor { get; private set; }
        public double BackGroundStartPointOffset { get; private set; }
        public string BackGroundEndPointColor { get; private set; }
        public double BackGroundEndPointOffset { get; private set; }
        public string PermissionSet { get; private set; } //构造方法1

        private readonly XDocument _document = XDocument.Load("GlobalSet.xml");
        int _errorNum = 0;
        private readonly StringBuilder _errorMessage = new StringBuilder();
        #endregion

        // ToDo 错误处理

        /// <summary>
        /// 获取设置
        /// </summary>
        /// <returns></returns>
        public override void GetData()
        {
            //获取连接字符串
            var DataBaseConfigGroup = _document.Element("database_config");
            var DataBaseSet = GetAllConStrSet(DataBaseConfigGroup);
            ConnectionDriver = DataBaseSet[0];
            ConnectionString = DataBaseSet[1];
            //获取Logo以及Logo背景
            var LoginFormSet = _document.Element("loginform_config");
            LogoPath = LogoSrcGet(LoginFormSet);
            var bgData = GetBackGround(PermissionSet);
            BackGroundPointSet(bgData);
        }

        /// <summary>
        /// 获取背景渐变端点
        /// </summary>
        /// <param name="bgData"></param>
        public void BackGroundPointSet(string[] bgData)
        {
            BackGroundStartPointColor = bgData[0];
            if (double.TryParse(bgData[1], out var BgSpo) == true)
            {
                BackGroundStartPointOffset = BgSpo;
            }
            else
            {
                _errorNum += 1;
                _errorMessage.Append("BackGroundStartPointOffset设置有误." + Environment.NewLine);
            }
            BackGroundEndPointColor = bgData[2];
            if (double.TryParse(bgData[3], out var BgEpo) == true)
            {
                BackGroundEndPointOffset = BgEpo;
            }
            else
            {
                _errorNum += 1;
                _errorMessage.Append("BackGroundEndPointOffset设置有误" + Environment.NewLine);
            }
        }

        /// <summary>
        /// 获取Logo存储路径
        /// </summary>
        /// <param name="loginFromSet"></param>
        /// <returns></returns>
        public string LogoSrcGet(XContainer loginFromSet)
        {
            try
            {
                LogoPath = (string)loginFromSet.Element("logo").Attribute("src");
                return LogoPath;
            }
            catch (NullReferenceException)
            {
                _errorNum += 1;
                _errorMessage.Append("loginfrom_config设置有误" + Environment.NewLine);
                return null;
            }
        }

        /// <summary>
        /// 获取连接驱动与连接字符串
        /// </summary>
        /// <param name="xElement"></param>
        /// <returns></returns>
        private string[] GetAllConStrSet(XContainer xElement)
        {
            const string xmlName = "dataconnectionstring";
            foreach (var Config in xElement.Descendants())
            {
                while (Config.HasAttributes && Config.Attribute("status").Value.Equals("enable"))
                {
                    if (GetDriver(Config.Element(xmlName)).Equals("ODBC") == true)
                    {
                        string[] result = new string[] { GetDriver(Config.Element(xmlName)), GetConStrOdbc(Config.Element(xmlName)) };
                        return result;
                    }
                    else
                    {
                        string[] result = new string[] { GetDriver(Config.Element(xmlName)), GetConStrAdo(Config.Element(xmlName)) };
                        return result;
                    }
                }
            }

            try
            {
                var DefaultConfig = xElement.Element("default").Element(xmlName);
                if (GetDriver(DefaultConfig).Equals("ODBC") == true)
                {
                    string[] result = new string[] {GetDriver(DefaultConfig), GetConStrOdbc(DefaultConfig)};
                    return result;
                }
                else
                {
                    string[] result = new string[] {GetDriver(DefaultConfig), GetConStrAdo(DefaultConfig)};
                    return result;
                }
            }
            catch (NullReferenceException)
            {
                _errorNum += 1;
                _errorMessage.Append("default设置组错误");
                return null;
            }
        }

        /// <summary>
        /// 生成ADO.NET驱动所需要的连接字符串
        /// </summary>
        /// <param name="xElement"></param>
        /// <returns></returns>
        private static string GetConStrAdo(XContainer xElement) 
        {
            var ea = xElement.Element("dataconnectionstring");
            var Server = (string)ea.Attribute("server");
            var DataBase = (string)ea.Attribute("database");
            var User = (string)ea.Attribute("usr");
            var Pwd = (string)ea.Attribute("pwd");
            var ConStr = $"server={Server};user={User};pwd={Pwd};database={DataBase}";
            return ConStr;
        }

        /// <summary>
        /// 生成ODBC驱动所需要的连接字符串
        /// </summary>
        /// <param name="xElement"></param>
        /// <returns></returns>
        private static string GetConStrOdbc(XContainer xElement)
        {
            var ea = xElement.Element("dataconnectionstring");
            var Dsn = (string)ea.Attribute("dsn");
            var DataBase = (string)ea.Attribute("database");
            var User = (string)ea.Attribute("usr");
            var Pwd = (string)ea.Attribute("pwd");
            var ConStr = $"DSN={Dsn};Uid={User};Pwd={Pwd};DataBase={DataBase}";
            return ConStr;
        }

        /// <summary>
        /// 获取连接驱动
        /// </summary>
        /// <param name="xElement"></param>
        /// <returns></returns>
        private static string GetDriver(XContainer xElement)
        {
            XElement ea = xElement.Element("dataconnectionstring");
            string Driver = (string)ea.Attribute("Driver");
            return Driver;
        }

        /// <summary>
        /// 保存设置,该类不启用
        /// </summary>
        public override void SetData()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取Logo背景渐变颜色
        /// </summary>
        /// <param name="pmsGroup"></param>
        /// <returns></returns>
        public string[] GetBackGround(string pmsGroup)
        {
            XElement LoginBgSet = _document.Element("loginform_config");
            var PmsBgSet = LoginBgSet.Element("background").Element(pmsGroup);
            var result = new string[] {
                (string)PmsBgSet.Element("startpoint").Attribute("color"), 
                (string)PmsBgSet.Element("startpoint").Attribute("offset"), 
                (string)PmsBgSet.Element("endpoint").Attribute("color"),
                (string)PmsBgSet.Element("endpoint").Attribute("offset") 
            };
            return result;
        }

        /// <summary>
        /// 获取默认权限
        /// </summary>
        /// <returns>默认权限</returns>
        public string GetPermission()
        {
            var Pms = _document.Element("defaultloginData");
            var result = (string)Pms.Element("permission").Attribute("default");
            return result;
        }

    }
}
