using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;

namespace StickyNote
{
    /// <summary>
    /// Setting.xaml 的交互逻辑
    /// 该窗口的作用：设置背景颜色和文字颜色
    /// </summary>
    public partial class Setting : Window
    {
        /// <summary>
        /// 子窗口向父窗口传值
        /// </summary>
        /// <param name="Color"></param>
        public delegate void SendMessage(string[] Color);
        public SendMessage sendMessage;
        //public string Path = System.IO.Directory.GetCurrentDirectory() + "/configure.xml";
        /// <summary>
        /// 窗口初始化
        /// </summary>
        /// <param name="top">上方</param>
        /// <param name="left">左方</param>
        public Setting(double top,double left)
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.Manual;
            this.Top = top;
            this.Left = left;
            //string ISCheacked = ReadXml();
            //if (ISCheacked == "true")
            //{
            //    Is_Auto.IsChecked = true;
            //}
            //else if (ISCheacked == "false")
            //{
            //    Is_Auto.IsChecked = false;
            //}
            
            #region 添加按钮点击事件
            
            Radio1.Checked += new RoutedEventHandler(radio_Checked);
            Radio2.Checked += new RoutedEventHandler(radio_Checked);
            Radio3.Checked += new RoutedEventHandler(radio_Checked);
            Radio4.Checked += new RoutedEventHandler(radio_Checked);
            Radio5.Checked += new RoutedEventHandler(radio_Checked);
            Radio6.Checked += new RoutedEventHandler(radio_Checked);
            Radio7.Checked += new RoutedEventHandler(radio_Checked);
            Radio8.Checked += new RoutedEventHandler(radio_Checked);
            Radio9.Checked += new RoutedEventHandler(radio_Checked);

            Radio11.Checked += new RoutedEventHandler(radio_Checked);
            Radio21.Checked += new RoutedEventHandler(radio_Checked);
            Radio31.Checked += new RoutedEventHandler(radio_Checked);
            Radio41.Checked += new RoutedEventHandler(radio_Checked);
            Radio51.Checked += new RoutedEventHandler(radio_Checked);
            Radio61.Checked += new RoutedEventHandler(radio_Checked);
            Radio71.Checked += new RoutedEventHandler(radio_Checked);
            Radio81.Checked += new RoutedEventHandler(radio_Checked);
            Radio91.Checked += new RoutedEventHandler(radio_Checked);

            #endregion 

        }

        /// <summary>
        /// 向xml中写入配置信息
        /// </summary>
        //private void WriteXml()
        //{
        //    //try
        //    //{
        //    //    if (!CheckXml("Signal"))
        //    //    {
        //    //        //MessageBox.Show("exist");
        //    //        //XmlDocument xmlDoc = new XmlDocument();
        //    //        //xmlDoc.Load(Path);
        //    //        //XmlNode root = xmlDoc.SelectSingleNode("ListBoxItem");
        //    //        //XmlElement xel = xmlDoc.CreateElement("Signal");
        //    //        //xel.SetAttribute("id", "-1");
        //    //        //XmlElement xesub1 = xmlDoc.CreateElement("autostart");
        //    //        //if (Is_Auto.IsChecked == true)
        //    //        //{
        //    //        //    MessageBox.Show("true1");
        //    //        //    xesub1.InnerText = "true";
        //    //        //}
        //    //        //else
        //    //        //{
        //    //        //    MessageBox.Show("false1");
        //    //        //    xesub1.InnerText = "false";
        //    //        //}

        //    //        //xel.AppendChild(xesub1);
        //    //        //root.AppendChild(xel);
        //    //        //xmlDoc.Save(Path);
        //    //    }
        //    //    else
        //    //    {
        //    //        MessageBox.Show("ext111");
        //    //        XmlDocument xmlDoc = new XmlDocument();
        //    //        xmlDoc.Load(Path);//加载xml文件，文件
        //    //        XmlNode xns = xmlDoc.SelectSingleNode("Signal");//查找要修改的节点
        //    //        XmlNodeList xnl = xns.ChildNodes;//取出所有的子节点
        //    //        foreach (XmlNode xn in xnl)
        //    //        {
        //    //            XmlElement xe = (XmlElement)xn;//将节点转换一下类型
        //    //            //if (xe.GetAttribute("id") == "-1")//判断该子节点是否是要查找的节点
        //    //            //{
        //    //            //    xe.SetAttribute("类别", "娱乐");//设置新值
        //    //            //}
        //    //            //else//为了有更明显的效果，所以不管是否是符合条件的子节点，我都给一个操作
        //    //            //{
        //    //            //    xe.SetAttribute("类别", "文学");
        //    //            //}

        //    //            XmlNodeList xnl2 = xe.ChildNodes;//取出该子节点下面的所有元素
        //    //            foreach (XmlNode xn2 in xnl2)
        //    //            {
        //    //                XmlElement xe2 = (XmlElement)xn2;//转换类型
        //    //                if (xe2.Name == "autostart")//判断是否是要查找的元素
        //    //                {
        //    //                    if (Is_Auto.IsChecked == true)
        //    //                    {
        //    //                        xe2.InnerText = "true";
        //    //                    }
        //    //                    else
        //    //                    {
                                    
        //    //                        xe2.InnerText = "false";
        //    //                    }

        //    //                }
        //    //            }
        //    //        }
        //    //        xmlDoc.Save(Path);

        //    //    }
        //    //    //XmlDocument doc = new XmlDocument();
        //    //    //doc.Load(Path);
        //    //    //XmlNodeList lis = doc.GetElementsByTagName("autostart");
        //    //    //lis[0].InnerText = ReadXml();
        //    //}
        //    //catch
        //    //{

        //    //}
            
        //}

        //public static bool CheckXml(string node)
        //{
        //    try
        //    {
        //        XmlDocument Checker = new XmlDocument();
        //        Checker.Load(System.IO.Directory.GetCurrentDirectory() + "/configure.xml");
        //        XmlNodeList HeaderList = Checker.DocumentElement.ChildNodes;
        //        foreach (XmlElement element in HeaderList)
        //        {
        //            if (element.Name == node)
        //            {
        //                return true;
        //            }
        //        }
        //    }
        //    catch
        //    {

        //    }

        //    //MessageBox.Show(HeaderList[1].ToString());
        //    return false;

           
        //}
        /// <summary>
        /// 读取xml
        /// </summary>
        /// <returns></returns>
        //private string ReadXml()
        //{
        //    try
        //    {
        //        XmlDocument doc = new XmlDocument();
        //        doc.Load(Path);
        //        XmlNodeList lis = doc.GetElementsByTagName("autostart");
        //        string str = lis[0].InnerText;
        //        //MessageBox.Show("12345");
        //        return str;
        //    }
        //    catch
        //    {
        //        return "false";
        //    }
        //}

        string[] Color = new string[2] {
            "#00000000","#FF0092BC"};

        /// <summary>
        /// 控制窗口关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Image3_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //WriteXml();
            this.Close();
        }

        /// <summary>
        /// 鼠标控制窗口移动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
        
        /// <summary>
        /// 按钮点击事件 向主窗口传递参数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radio_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton btn = sender as RadioButton;
            if (btn == null)
                return;
            switch (btn.Name)
            {
                case "Radio1":
                    Color[0] = ((Brush)Radio1.Background).ToString();
                    Color[1] = "null";
                    break;
                case "Radio2":
                    Color[0] = ((Brush)Radio2.Background).ToString();
                    Color[1] = "null";
                    break;

                case "Radio3":
                    Color[0] = ((Brush)Radio3.Background).ToString();
                    Color[1] = "null";
                    break;
                case "Radio4":
                    Color[0] = ((Brush)Radio4.Background).ToString();
                    Color[1] = "null";
                    break;
                case "Radio5":
                    Color[0] = ((Brush)Radio5.Background).ToString();
                    Color[1] = "null";
                    break;
                case "Radio6":
                    Color[0] = ((Brush)Radio6.Background).ToString();
                    Color[1] = "null";
                    break;
                case "Radio7":
                    Color[0] = ((Brush)Radio7.Background).ToString();
                    Color[1] = "null";
                    break;
                case "Radio8":
                    Color[0] = ((Brush)Radio8.Background).ToString();
                    Color[1] = "null";
                    break;
                case "Radio9":
                    Color[0] = ((Brush)Radio9.Background).ToString();
                    Color[1] = "null";
                    break;
                case "Radio11":
                    Color[1] = ((Brush)Radio11.Background).ToString();
                    Color[0] = "null";
                    break;
                case "Radio21":
                    Color[1] = ((Brush)Radio21.Background).ToString();
                    Color[0] = "null";
                    break;
                case "Radio31":
                    Color[1] = ((Brush)Radio31.Background).ToString();
                    Color[0] = "null";
                    break;
                case "Radio41":
                    Color[1] = ((Brush)Radio41.Background).ToString();
                    Color[0] = "null";
                    break;
                case "Radio51":
                    Color[1] = ((Brush)Radio51.Background).ToString();
                    Color[0] = "null";
                    break;
                case "Radio61":
                    Color[1] = ((Brush)Radio61.Background).ToString();
                    Color[0] = "null";
                    break;
                case "Radio71":
                    Color[1] = ((Brush)Radio71.Background).ToString();
                    Color[0] = "null";
                    break;
                case "Radio81":
                    Color[1] = ((Brush)Radio81.Background).ToString();
                    Color[0] = "null";
                    break;
                case "Radio91":
                    Color[1] = "默认";
                    Color[0] = "null";
                    break;
                default:
                    Color[0] = "null";
                    Color[1] = "null";
                    break;
            }
            sendMessage(Color);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //WriteXml();
        }
    }
}