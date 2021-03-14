using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;
using IWshRuntimeLibrary;
//using System.Windows.Forms;

using System.ComponentModel;
using Drawing = System.Drawing;
namespace StickyNote
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private System.Windows.Forms.NotifyIcon notifyIcon;
        public string XmlPath = System.IO.Directory.GetCurrentDirectory() + "/configure.xml";  // 定义Xml文件路径，使其与exe文件处于同一目录下
        public MainWindow()
        {
            InitializeComponent();

            #region 设置主窗口的初始位置
            WindowStartupLocation = WindowStartupLocation.Manual;
            this.Top = 20;
            this.Left = SystemParameters.WorkArea.Width-280;
            #endregion

            InitNotifyIcon();  //初始化托盘图标 

            #region 设置是否开机自启和初始化
            if (ReadXml() == "true")
            {
                YesImage.Source = new BitmapImage(new Uri("pack://application:,,,/resources/yes.png"));
                StartAutomaticallyCreate("StickyNote");
            }
            else
            {
                YesImage.Source = null;
                StartAutomaticallyDel("StickyNote");
            }
            #endregion

            #region 设置是否弹出关闭弹窗
            if (ReadClose() == "true")
            {
                CloseImage.Source = new BitmapImage(new Uri("pack://application:,,,/resources/yes.png"));
                
            }
            else
            {
                CloseImage.Source = null;
            }
            #endregion

            Dater.Text = DateTime.Today.ToString();  // 日期选择框初始化
            ReadFile();  // 读取文件初始化
        }
        private void InitNotifyIcon()
        {
            this.notifyIcon = new System.Windows.Forms.NotifyIcon();
            this.notifyIcon.BalloonTipText = "StickNote";
            this.notifyIcon.ShowBalloonTip(2000);
            this.notifyIcon.Text = "StickNote";
            this.notifyIcon.Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath);
            this.notifyIcon.Visible = true;
            //打开菜单项
            System.Windows.Forms.MenuItem open = new System.Windows.Forms.MenuItem("显示");
            open.Click += new EventHandler(Show);
            //退出菜单项
            System.Windows.Forms.MenuItem exit = new System.Windows.Forms.MenuItem("退出");
            exit.Click += new EventHandler(Close);
            //关联托盘控件
            System.Windows.Forms.MenuItem[] childen = new System.Windows.Forms.MenuItem[] { open, exit };
            notifyIcon.ContextMenu = new System.Windows.Forms.ContextMenu(childen);
            this.notifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler((o, e) =>
            {
                if (e.Button == System.Windows.Forms.MouseButtons.Left) this.Show(o, e);
            });
        }
        private void Show(object sender, EventArgs e)
        {
            this.Visibility = System.Windows.Visibility.Visible;
            this.WindowState = System.Windows.WindowState.Normal;
            this.ShowInTaskbar = true;
            this.Activate();
        }
        private void Hide(object sender, EventArgs e)
        {
            this.ShowInTaskbar = false;
            this.Visibility = System.Windows.Visibility.Hidden;
            this.WindowState = System.Windows.WindowState.Minimized;
        }
        private void Close(object sender, EventArgs e)
        {
            SaveFile();
            this.notifyIcon.Visible = false;
            System.Windows.Application.Current.Shutdown();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.ShowInTaskbar = false;
            this.WindowState = WindowState.Minimized;
            e.Cancel = true;
        }
        
        /// <summary>
        /// 自定义窗口 Begin
        /// </summary>

        bool isWiden = false;
        double LeftLocation = 0;
        double Right = 0;
        double wide = 0;
        /// <summary>
        /// 获得窗口初始宽度
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void window_initiateWiden(object sender, System.Windows.Input.MouseEventArgs e)
        {
            LeftLocation = this.Left;
            Right = this.Left + this.Width;
            isWiden = true;
        }
        /// <summary>
        /// 获得窗口鼠标松开后
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void window_endWiden(object sender, System.Windows.Input.MouseEventArgs e)
        {
            isWiden = false;
            // Make sure capture is released.
            Rectangle rect = (Rectangle)sender;
            rect.ReleaseMouseCapture();
        }

        /// <summary>
        /// 向右拉伸窗口改变窗口宽度
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void window_Widen(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Rectangle rect = (Rectangle)sender;
            if (isWiden)
            {
                rect.CaptureMouse();
                double newWidth = e.GetPosition(this).X + 5;
                if (newWidth > 220) this.Width = newWidth;
            }
        }
        /// <summary>
        /// 向左拉伸窗口改变窗口宽度
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void window_WidenL(object sender, System.Windows.Input.MouseEventArgs e)
        {

            Rectangle rect = (Rectangle)sender;
            if (isWiden)
            {
                rect.CaptureMouse();
                double newLocation = e.GetPosition(this).X + LeftLocation;
                wide = Right - newLocation - 5;
                if (newLocation > 0 && wide >= 220)
                {
                    this.Width = wide;
                    this.Left = newLocation;
                }
            }
        }

        bool isHeight = false;
        /// <summary>
        /// 获得初始窗口高度
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void window_initiateHeight(object sender, System.Windows.Input.MouseEventArgs e)
        {
            isHeight = true;
        }
        private void window_endHeight(object sender, System.Windows.Input.MouseEventArgs e)
        {
            isHeight = false;

            // Make sure capture is released.
            Rectangle rect = (Rectangle)sender;
            rect.ReleaseMouseCapture();
        }
        /// <summary>
        /// 改变窗口高度
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void window_Height(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Rectangle rect = (Rectangle)sender;
            if (isHeight)
            {
                rect.CaptureMouse();
                double newHeight = e.GetPosition(this).Y + 5;
                if (newHeight > 200) this.Height = newHeight;
            }
        }
        /// <summary>
        /// 拖拽窗口移动
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
        /// 窗口最小化及窗口关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Image1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Image3_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SaveFile();

            if (ReadClose() == "true")
            {
                if (ReadJust() == "true")
                {
                    notifyIcon.Visible = false;
                    this.Close();
                }
                else
                {
                    this.ShowInTaskbar = false;
                    this.Visibility = Visibility.Collapsed;
                }
               
            }
            else
            {
                CloseMessage closeMessage = new CloseMessage()
                {
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                };
                closeMessage.sendMessage = CloseWindow;
                closeMessage.ShowDialog();
            }
           
        }

        public void CloseWindow(string[] CloseMg)
        {
            if (CloseMg[1] == "true")
            {
                ReviseClose("true");

            }
            else
            {
                ReviseClose("false");
            }
            #region 设置是否弹出关闭弹窗
            if (ReadClose() == "true")
            {
                CloseImage.Source = new BitmapImage(new Uri("pack://application:,,,/resources/yes.png"));

            }
            else
            {
                CloseImage.Source = null;
            }
            #endregion
            if (CloseMg[0] == "Mini")
            {
                ReviseJust("false");
                this.ShowInTaskbar = false;
                this.Visibility = Visibility.Collapsed;
            }
            else
            {
                ReviseJust("true");
                this.notifyIcon.Visible = false;
                this.Close();
            }
        }
        /// <summary>
        /// 生成并返回字体颜色画笔资源
        /// </summary>
        /// <returns></returns>
        public LinearGradientBrush MylinearGradientBrush()
        {
            LinearGradientBrush brush = new LinearGradientBrush();

            GradientStop gs1 = new GradientStop();
            gs1.Offset = 0;
            gs1.Color = Color.FromArgb(255,0,143,108);
            brush.GradientStops.Add(gs1);

            GradientStop gs2 = new GradientStop();
            gs2.Offset = 0.5;
            gs2.Color = Color.FromArgb(255,24,27,175);
            brush.GradientStops.Add(gs2);

            return brush;
        }
        /// <summary>
        /// 图片文件声明
        /// </summary>
        private string[] Image = new string[]
        {
            "pack://application:,,,/resources/study.png",
            "pack://application:,,,/resources/work.png",
            "pack://application:,,,/resources/life.png",
            "pack://application:,,,/resources/joy.png",
            "pack://application:,,,/resources/other.png"
        };


        public List<Item> ItemList { get; set; }
        Item deleteItem = new Item();
        /// <summary>
        /// ListBox中的listItem 自定义类声明
        /// </summary>
        public class Item
        {
            public System.Windows.Media.Brush GridBack { get; set; }
            public System.Windows.Media.Brush FontColor { get; set; }
            public ImageSource ImageSe { get; set; }
            public string Info { get; set; }
            public string Time { get; set; }
            public int PicIndex { get; set; }  // 用于标记图片资源
            //public string SeeBox { get; set; }
            //public Visibility See { get; set; }
            //public string NewInfo { get; set; }
            //public string NewTime { get; set; }
        }

        /// <summary>
        /// 鼠标选中后使用delete键删除元素
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MyListBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Delete && MyListBox.SelectedIndex != -1)
            {
                if (!MyListBox.Items.IsEmpty)
                {
                    deleteItem = (Item)MyListBox.SelectedItem;
                    ItemList.Remove(deleteItem);
                    FileFlash();
                }
            }
        }

        public Brush IndexColor=new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFFFF")); // 存储画笔资源
        public string IndexBrush = "null";
        /// <summary>
        /// 获取子窗口返回的字符串，用于改变主窗口的背景颜色和文字颜色
        /// </summary>
        /// <param name="Color"></param>
        public void Recevie(string[] Color)
        {
            Color color;
            try
            {
                if (Color[0] != "null")
                {
                    color = (Color)ColorConverter.ConvertFromString(Color[0]);
                    border.Background = new SolidColorBrush(color);
                }

            }
            catch
            {
                MessageBox.Show("Something Wrong");
                color = (Color)ColorConverter.ConvertFromString("#00000000");
                border.Background = new SolidColorBrush(color);
            }

            try
            {

                if (Color[1] != "默认" && Color[1] != "null")
                {
                    for (int i = 0; i < ItemList.Count; i++)
                    {
                        ItemList[i].FontColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Color[1]));
                        
                    }
                }
                else if (Color[1] != "null")
                {
                    for (int i = 0; i < ItemList.Count; i++)
                    {
                        ItemList[i].FontColor = MylinearGradientBrush();
                    }
                }
                IndexBrush = Color[1];
                FileFlash();
                //MyListBox.ItemsSource = null;
                //MyListBox.ItemsSource = ItemList;
                //Border2.BorderBrush = new SolidColorBrush(color);
            }
            catch
            {

                MessageBox.Show("Something Woring goes in the border Color");
                //color = (Color)ColorConverter.ConvertFromString("#FF0092BC");
                //Border2.BorderBrush = new SolidColorBrush(color);
            }

            //try
            //{

            //    color = (Color)ColorConverter.ConvertFromString(Color[2]);
            //    //TextBlock1.Foreground = new SolidColorBrush(color);
            //}
            //catch
            //{
            //    //MessageBox.Show("Something Woring goes in the Font Color");
            //    color = (Color)ColorConverter.ConvertFromString("#FF277AD4");
            //    //TextBlock1.Foreground = new SolidColorBrush(color);
            //}
        }

        /// <summary>
        /// 保存Xml文件
        /// </summary>
        private void SaveFile()
        {
            Item item = new Item();
            string str = ReadXml();
            string closeM = ReadClose();
            string JustM = ReadJust();

            XmlTextWriter writer = new XmlTextWriter(XmlPath, System.Text.Encoding.UTF8);
            
            writer.Formatting = Formatting.Indented;
            writer.WriteStartDocument(); //XML声明 
            writer.WriteStartElement("ListBoxItem");
            for (int i = 0; i < ItemList.Count; i++)
            {
                item = ItemList[i];
                writer.WriteStartElement("item");
                writer.WriteAttributeString("id", i.ToString());
                writer.WriteElementString("info", item.Info);
                writer.WriteElementString("time", item.Time);
                writer.WriteElementString("PicIndex", item.PicIndex.ToString());
                //关闭item元素
                writer.WriteEndElement(); // 关闭元素
            }

            writer.WriteStartElement("Signal");
            writer.WriteAttributeString("id", "-1");
            writer.WriteElementString("auto", str);
            writer.WriteEndElement();

            writer.WriteStartElement("Close");
            writer.WriteAttributeString("id", "-2");
            writer.WriteElementString("Message", closeM);
            writer.WriteEndElement();

            writer.WriteStartElement("Just");
            writer.WriteAttributeString("id", "-3");
            writer.WriteElementString("MessageJust",JustM);
            writer.WriteEndElement();
            
            writer.Close();
        }

        private string ReadJust()
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(XmlPath);
                XmlNodeList lis = doc.GetElementsByTagName("MessageJust");
                string str = lis[lis.Count - 1].InnerText;
                return str;
            }
            catch
            {
                return "false";
            }
        }

        /// <summary>
        /// 读取Xml文件
        /// </summary>
        private void ReadFile()
        {
            List<Item> TempItemList = new List<Item>();
            XmlDocument Reader = new XmlDocument();
            bool IS_Exit = false;
            try
            {
                Reader.Load(XmlPath);

                XmlNodeList ListInfo = Reader.GetElementsByTagName("info");
                XmlNodeList ListTime = Reader.GetElementsByTagName("time");
                XmlNodeList ListIndex = Reader.GetElementsByTagName("PicIndex");
                
                for (int i = 0; i < ListIndex.Count; i++)
                {
                    if (int.Parse(ListIndex[i].InnerText) == -1)
                    {
                        ListIndex[i].InnerText = "4";
                    }
                    Item newitem = new Item()
                    {
                        ImageSe = new BitmapImage(new Uri(Image[int.Parse(ListIndex[i].InnerText)])),
                        FontColor = MylinearGradientBrush(),
                        Info = ListInfo[i].InnerText,
                        Time = ListTime[i].InnerText,
                        PicIndex = int.Parse(ListIndex[i].InnerText)
                    };
                    TempItemList.Add(newitem);
                }
                IS_Exit = true;
            }
            catch
            {
                Item item = new Item()
                {
                    ImageSe = new BitmapImage(new Uri(Image[4])),
                    FontColor = MylinearGradientBrush(),
                    Info = "Welcome",
                    Time = DateTime.Now.ToString("yyyy/MM/dd hh:mm"),
                    PicIndex = 4
                };
                TempItemList.Add(item);

            }
            ItemList = TempItemList;
            if (!IS_Exit)
            {
                //MessageBox.Show("there is no xml");
                SaveFile();
            }
            

            FileFlash();
        }

        ///// <summary>
        ///// 用于判断是否含有某一个节点
        ///// </summary>
        ///// <param name="node"></param>
        ///// <returns>(bool) true or false</returns>
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
        //                MessageBox.Show("find it");
        //                return true;
        //            }
        //        }
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //    return false;
        //}

        /// <summary>
        /// 刷新列表框
        /// </summary>
        private void FileFlash()
        {
            MyListBox.ItemsSource = null;
            MyListBox.ItemsSource = ItemList;
        }

        /// <summary>
        /// 添加列表元素（即添加待办事项）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string Text = ItemText.Text;
            string YearM = Convert.ToDateTime(Dater.Text).ToString("yyyy/MM/dd");
            string HourM = " 00:00";
            if (hour.Text.Length != 0 && minute.Text.Length != 0)
            {
                HourM = " " + hour.Text + ":" + minute.Text;
            }
            else if (hour.Text.Length != 0 && minute.Text.Length == 0)
            {
                HourM = " " + hour.Text + ":" + DateTime.Now.Minute;
            }
            else if (hour.Text.Length == 0 && minute.Text.Length != 0)
            {
                HourM = " " + DateTime.Now.Hour.ToString() + ":" + minute.Text;
            }
            else
            {
                HourM = " 时间未定";
            }
            string TimeT = YearM + HourM;
            try
            {
                if (int.Parse(hour.Text) > 24 || (int.Parse(hour.Text) > 60))
                {
                    TimeT = " 每天";
                }
            }
            catch
            {

            }
            
            
            int index = comboBox.SelectedIndex;
            if (index == -1)
            {
                index = 4;
            }

            Item newitem = new Item()
            {
                ImageSe = new BitmapImage(new Uri(Image[index])),
                FontColor = MylinearGradientBrush(),
                Info = Text,
                Time = TimeT,
                //See = Visibility.Collapsed,
                //NewInfo = Text,
                //NewTime = TimeT,
                PicIndex = index
            };
            
            if (IndexBrush != "null")
            {
                try
                {
                    newitem.FontColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(IndexBrush));
                }
                catch
                {
                    MessageBox.Show("");
                    newitem.FontColor = MylinearGradientBrush();
                }
               
            }
            ItemList.Add(newitem);
            ItemText.Text = null;
            hour.Text = null;
            minute.Text = null;
            
            comboBox.SelectedIndex = index;
            FileFlash();
            SaveFile();
        }
        /// <summary>
        /// 控制输入框只能输入0-9数字
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex re = new Regex("[^0-9]+");
            e.Handled = re.IsMatch(e.Text);
        }

        private int SelectIndex = 0;

        public void GetInfo(string[] InfoT)
        {
            ItemList[SelectIndex].Info = InfoT[0];
            ItemList[SelectIndex].Time = InfoT[1];
            FileFlash();
        }
        /// <summary>
        /// 双击显示具体截止时间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MyListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (MyListBox.SelectedIndex != -1)
            {
                if (!MyListBox.Items.IsEmpty)
                {
                    Point point = e.GetPosition(null);
                    double X = 30 + this.Left;
                    double Y = point.Y + this.Top + 15;
                    //MessageBox.Show(point.X.ToString());
                    SelectIndex = MyListBox.SelectedIndex;
                    MyMessageBox myMessageBox = new MyMessageBox(Y,X);
                    myMessageBox.sendMessage = GetInfo;
                    myMessageBox.ShowDialog();
                    
                }
            }
        }

        /// <summary>
        /// 弹出选择颜色的对话框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Image2_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Setting setting = new Setting(this.Top, this.Left);
            setting.sendMessage = Recevie;
            setting.ShowDialog();
        }

        /// <summary>
        /// 不同格式文件的编写
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private string FileParse(string type)
        {
            string plaintext = " ";
            string mdtext = " ";
            string htmltext = " ";
            string errortext = "An error may have occurred, please try again!";
            if (type == "txt")
            {
                plaintext = "本文件于 " + DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss") + " 创建\n";
                plaintext += "=====================================================\n";
                plaintext += "序号" + "\t" + "待办事项" + "\t\t" + "截止时间\n";

                for (int i = 0; i < ItemList.Count; i++)
                {
                    plaintext += (i + 1).ToString() + "\t" + ItemList[i].Info + "\t\t" + ItemList[i].Time + "\n";
                }
                return plaintext;
            }
            else if (type == "md")
            {
                mdtext = "### 本文件于`" + DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss") + "`创建\n";
                mdtext += "***\n";
                mdtext += "| 序号|待办事项 |截止时间|\n";
                mdtext += ":-:|:-:|:-:\n";
                for (int i = 0; i < ItemList.Count; i++)
                {
                    mdtext += (i + 1).ToString() + "|" + ItemList[i].Info + "|" + ItemList[i].Time + "| \n";
                }
                return mdtext;
            }
            else if (type == "html")
            {
                htmltext = "<!DOCTYPE html><html><head><meta charset=\"utf-8\"><meta name=\"viewport\" content=\"width = device-width, initial-scale = 1\">\n" +
                "<style>body {max-width: 980px;border: 1px solid #ddd;outline: 1300px solid #fff;margin: 16px auto;}body.markdown-body{padding: 45px;}\n" +
                ".markdown-body table{border-collapse: collapse;border-spacing: 0;}.markdown-body td,.markdown-body th{padding: 0;}.markdown-body h4{font-size: 1.3em;}\n" +
                ".markdown-body table{margin-top: 0;margin-bottom: 16px;}.markdown-body hr{height: 6px;padding: 0; margin: 16px 0; background-color: #e7e7e7;border: 0 none;}\n" +
                ".markdown-body table{display: block;width: 100%;overflow: auto;word-break: normal; word-break: keep-all;}.markdown-body table th {font-weight: bold;}\n" +
                ".markdown-body table th,.markdown-body table td {padding: 6px 13px;border: 1px solid #ddd;}.markdown-body table tr { background-color: #fff;border-top: 1px solid #ccc;}\n" +
                ".markdown-body table tr:nth-child(2n){ background-color: #f8f8f8;}.markdown-body code{ padding: 0; padding-top: 0.2em; padding-bottom: 0.2em; margin: 0;font-size: 100%;background-color: rgba(0,0,0,0.04);border-radius: 3px;}\n" +
                ".markdown-body code:before,.markdown-body code:after { letter-spacing: -0.2em; content: \" \\00a0\";}</style>";
                htmltext += "<title>便笺</title></head>\n <body>\n";
                htmltext += " <article class=\"markdown-body\"><h2>本文件于<code>" + DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss") + "</code>创建</h2>";
                htmltext += "<hr/>\n<table> <thead><tr ><th align =\"center\" > 序号 </th ><th align = \"center\" > 待办事项 </th ><th align = \"center\" > 截止时间 </th >\n</tr >\n</thead >\n <tbody > ";
                for (int i = 0; i < ItemList.Count; i++)
                {
                    htmltext += "<tr>\n <td align=\"center\">" + (i + 1).ToString() + "</td>\n<td align=\"center\">" + ItemList[i].Info + "</td>\n<td align=\"center\">" + ItemList[i].Time + "</td>\n</tr>\n";
                }
                htmltext += "</tbody></article ></body ></html > ";
                return htmltext;
            }
            else
            {
                return errortext;
            }
            //return plaintext;
        }

        /// <summary>
        /// 导出待办事项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "文本文件(*.txt )|*.txt |标记文档(*.md)|*.md|网页(*.html)|*.html";
            sfd.RestoreDirectory = true;//保存对话框是否记忆上次打开的目录
            if (sfd.ShowDialog() == true)
            {
                string filename = sfd.FileName.ToString();
                string[] fileArray = filename.Split('.');

                if (fileArray[1] == "txt")
                {
                    StreamWriter FileWriter = new StreamWriter(sfd.FileName);
                    FileWriter.Write(FileParse("txt"));
                    FileWriter.Close();
                }
                else if (fileArray[1] == "md")
                {
                    StreamWriter FileWriter = new StreamWriter(sfd.FileName);
                    FileWriter.Write(FileParse("md"));
                    FileWriter.Close();
                }
                else if (fileArray[1] == "html")
                {
                    StreamWriter FileWriter = new StreamWriter(sfd.FileName);
                    FileWriter.Write(FileParse("html"));
                    FileWriter.Close();
                }

            }
        }

        #region 设置开机自启
        /// <summary>
        /// 
        /// </summary>
        /// <param name="exeName">程序名</param>
        /// <returns>bool</returns>
        public bool StartAutomaticallyCreate(string exeName)
        {
            try
            {
                WshShell shell = new WshShell();
                IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(Environment.GetFolderPath(Environment.SpecialFolder.Startup) + "\\" + exeName + ".lnk");
                //设置快捷方式的目标所在的位置(源程序完整路径) 
                shortcut.TargetPath = System.Windows.Forms.Application.ExecutablePath;
                //应用程序的工作目录 
                //当用户没有指定一个具体的目录时，快捷方式的目标应用程序将使用该属性所指定的目录来装载或保存文件。 
                shortcut.WorkingDirectory = System.Environment.CurrentDirectory;
                //MessageBox.Show(shortcut.WorkingDirectory);
                //目标应用程序窗口类型(1.Normal window普通窗口,3.Maximized最大化窗口,7.Minimized最小化) 
                shortcut.WindowStyle = 1;
                 //快捷方式的描述 
                shortcut.Description = exeName + "_Ink";
                //设置快捷键(如果有必要的话.) 
                 //shortcut.Hotkey = "CTRL+ALT+D"; 
                shortcut.Save();
                return true;
            }
            catch (Exception)
            {
                MessageBox.Show("设置开机自启时出现错误");
            }
              return false;
        }

        /// <summary>
        /// 获取当前开机自启的状态
        /// </summary>
        /// <returns>(string)"true" or "false"</returns>
        private string ReadXml()
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(XmlPath);
                XmlNodeList lis = doc.GetElementsByTagName("auto");
                string str = lis[lis.Count - 1].InnerText;
                return str;
            }
            catch
            {
                return "false";
            }
        }
        private string ReadClose()
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(XmlPath);
                XmlNodeList lis = doc.GetElementsByTagName("Message");
                string str = lis[lis.Count - 1].InnerText;
                return str;
            }
            catch
            {
                return "false";
            }
        }
        /// <summary>
        /// 开机自启删除
        /// </summary>
        /// <param name="exeName">程序名称</param>
        /// <returns></returns>
        public bool StartAutomaticallyDel(string exeName)
        {
            try
            {
                System.IO.File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.Startup) + "\\" + exeName + ".lnk");
                return true;
            }
            catch (Exception) { }
            return false;
            #endregion
        }

        /// <summary>
        /// 设置待办事项是否可见
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (MyListBox.Visibility == Visibility.Visible)
            {
                SeeImage.Source = new BitmapImage(new Uri("pack://application:,,,/resources/see.png"));
                InputStack.Visibility = Visibility.Collapsed;
                MyListBox.Visibility = Visibility.Collapsed;
                this.Height = 50;
            }
            else
            {
                SeeImage.Source = new BitmapImage(new Uri("pack://application:,,,/resources/nosee.png"));
                InputStack.Visibility = Visibility.Visible;
                MyListBox.Visibility = Visibility.Visible;
                this.Height = 300;
            }
        }

        /// <summary>
        /// 右键菜单删除定义
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            
            if (MyListBox.SelectedIndex != -1)
            {
                if (!MyListBox.Items.IsEmpty)
                {
                    deleteItem = (Item)MyListBox.SelectedItem;
                    ItemList.Remove(deleteItem);
                    FileFlash();
                }
            }
        }

        /// <summary>
        /// 右键菜单导出为txt格式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            if (MyListBox.SelectedIndex != -1)
            {
                if (!MyListBox.Items.IsEmpty)
                {
                    Item ExportItem = (Item)MyListBox.SelectedItem;
                    int num = MyListBox.SelectedIndex;
                    string plaintext = "本文件于 " + DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss") + " 创建\n";
                    plaintext += "=====================================================\n";
                    plaintext += "序号" + "\t" + "待办事项" + "\t\t" + "截止时间\n";

                    plaintext += num.ToString() + "\t" + ExportItem.Info + "\t\t" + ExportItem.Time + "\n";
                    
                    SaveFileDialog sfd = new SaveFileDialog();
                    sfd.Filter = "文本文件(*.txt )|*.txt";
                    sfd.RestoreDirectory = true;//保存对话框是否记忆上次打开的目录
                    if (sfd.ShowDialog() == true)
                    {
                        StreamWriter FileWriter = new StreamWriter(sfd.FileName);
                        FileWriter.Write(plaintext);
                        FileWriter.Close();
                    }
                }
            }
            
        }

        /// <summary>
        /// 右键菜单导出为md
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            if (MyListBox.SelectedIndex != -1)
            {
                if (!MyListBox.Items.IsEmpty)
                {
                    Item ExportItem = (Item)MyListBox.SelectedItem;
                    int num = MyListBox.SelectedIndex;
                    string mdtext = "### 本文件于`" + DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss") + "`创建\n";
                    mdtext += "***\n";
                    mdtext += "| 序号|待办事项 |截止时间|\n";
                    mdtext += ":-:|:-:|:-:\n";
                    mdtext += num.ToString() + "|" + ExportItem.Info + "|" + ExportItem.Time + "| \n";
                    SaveFileDialog sfd = new SaveFileDialog();
                    sfd.Filter = "标记文档(*.md)|*.md";
                    sfd.RestoreDirectory = true;//保存对话框是否记忆上次打开的目录
                    if (sfd.ShowDialog() == true)
                    {
                        StreamWriter FileWriter = new StreamWriter(sfd.FileName);
                        FileWriter.Write(mdtext);
                        FileWriter.Close();
                    }
                }
            }
        }

        /// <summary>
        /// 右键菜单导出为网页.html
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            if (MyListBox.SelectedIndex != -1)
            {
                if (!MyListBox.Items.IsEmpty)
                {
                    Item ExportItem = (Item)MyListBox.SelectedItem;
                    int num = MyListBox.SelectedIndex;
                    string htmltext = "<!DOCTYPE html><html><head><meta charset=\"utf-8\"><meta name=\"viewport\" content=\"width = device-width, initial-scale = 1\">\n" +
                "<style>body {max-width: 980px;border: 1px solid #ddd;outline: 1300px solid #fff;margin: 16px auto;}body.markdown-body{padding: 45px;}\n" +
                ".markdown-body table{border-collapse: collapse;border-spacing: 0;}.markdown-body td,.markdown-body th{padding: 0;}.markdown-body h4{font-size: 1.3em;}\n" +
                ".markdown-body table{margin-top: 0;margin-bottom: 16px;}.markdown-body hr{height: 6px;padding: 0; margin: 16px 0; background-color: #e7e7e7;border: 0 none;}\n" +
                ".markdown-body table{display: block;width: 100%;overflow: auto;word-break: normal; word-break: keep-all;}.markdown-body table th {font-weight: bold;}\n" +
                ".markdown-body table th,.markdown-body table td {padding: 6px 13px;border: 1px solid #ddd;}.markdown-body table tr { background-color: #fff;border-top: 1px solid #ccc;}\n" +
                ".markdown-body table tr:nth-child(2n){ background-color: #f8f8f8;}.markdown-body code{ padding: 0; padding-top: 0.2em; padding-bottom: 0.2em; margin: 0;font-size: 100%;background-color: rgba(0,0,0,0.04);border-radius: 3px;}\n" +
                ".markdown-body code:before,.markdown-body code:after { letter-spacing: -0.2em; content: \" \\00a0\";}</style>";
                    htmltext += "<title>便笺</title></head>\n <body>\n";
                    htmltext += " <article class=\"markdown-body\"><h2>本文件于<code>" + DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss") + "</code>创建</h2>";
                    htmltext += "<hr/>\n<table> <thead><tr ><th align =\"center\" > 序号 </th ><th align = \"center\" > 待办事项 </th ><th align = \"center\" > 截止时间 </th >\n</tr >\n</thead >\n <tbody > ";
                    
                    htmltext += "<tr>\n <td align=\"center\">" + num.ToString() + "</td>\n<td align=\"center\">" + ExportItem.Info + "</td>\n<td align=\"center\">" + ExportItem.Time + "</td>\n</tr>\n";
                   
                    htmltext += "</tbody></article ></body ></html > ";
                    SaveFileDialog sfd = new SaveFileDialog();
                    sfd.Filter = "网页文件(*.html)|*.html";
                    sfd.RestoreDirectory = true;//保存对话框是否记忆上次打开的目录
                    if (sfd.ShowDialog() == true)
                    {
                        StreamWriter FileWriter = new StreamWriter(sfd.FileName);
                        FileWriter.Write(htmltext);
                        FileWriter.Close();
                    }
                }
            }
        }

        /// <summary>
        /// 右键菜单详细信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click_4(object sender, RoutedEventArgs e)
        {
            if (MyListBox.SelectedIndex != -1)
            {
                if (!MyListBox.Items.IsEmpty)
                {
                    string InfoStr = " ";
                    Item InfoItem = (Item)MyListBox.SelectedItem;
                    InfoStr += "\t待办事项：" + InfoItem.Info + "\t\n\n" + "\t截止时间：" + InfoItem.Time+"\t";
                    MessageBox.Show(InfoStr);
                }
            }
            
        }

        /// <summary>
        /// 防止右键按下时在非列表项区域弹出右键菜单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MyListBox_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (MyListBox.SelectedIndex != -1)
            {
                if (!MyListBox.Items.IsEmpty)
                {
                    //使消息可以传递
                    e.Handled = false;
                }
                
            }
            else
            {
                e.Handled = true;
            }
            
        }

        /// <summary>
        /// 左键按下空白区域时取消对列表项的选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MyListBox_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (MyListBox.SelectedIndex != -1)
                {
                    if (!MyListBox.Items.IsEmpty)
                    {
                        MyListBox.SelectedIndex = -1;
                    }
                }
            }
            catch
            {
                MessageBox.Show("列表框在左键按下时出现未知错误");
            }
            
        }

        /// <summary>
        /// 右键按下非列表项区域时取消对列表项的选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void MyListBox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (MyListBox.SelectedIndex != -1)
                {

                    if (!MyListBox.Items.IsEmpty)
                    {
                        MyListBox.SelectedIndex = -1;
                    }

                }
            }
            catch
            {
                MessageBox.Show("列表框在右键按下时出现未知错误");
            }
            
        }
        /// <summary>
        /// 修改xml文件中的auto项
        /// </summary>
        /// <param name="Rev"></param>
        private void ReviseXml(string Rev)
        {
            try
            {
                XmlDocument XmlDoc = new XmlDocument();
                XmlDoc.Load(XmlPath);//加载xml文件，文件

                XmlNode xns = XmlDoc.SelectSingleNode("ListBoxItem");//查找要修改的节点

                XmlNodeList ChildNode = xns.ChildNodes;//取出所有的子节点

                foreach (XmlNode xn in ChildNode)
                {
                    XmlElement Xele = (XmlElement)xn;//将节点转换一下类型
                    if (Xele.GetAttribute("id") == "-1")//判断该子节点是否是要查找的节点
                    {
                        XmlNodeList Xncn = Xele.ChildNodes;//取出该子节点下面的所有元素
                        foreach (XmlNode xn2 in Xncn)
                        {
                            XmlElement Xele2 = (XmlElement)xn2;//转换类型
                            if (Xele2.Name == "auto")//判断是否是要查找的元素
                            {
                                Xele2.InnerText = Rev;
                            }
                        }
                    }
                  
                }
                XmlDoc.Save(XmlPath);//再一次强调 ，一定要记得保存的该XML文件
            }
            catch
            {

            }
            
           
        
    }
        private void ReviseClose(string Rev)
        {
            try
            {
                XmlDocument XmlDoc = new XmlDocument();
                XmlDoc.Load(XmlPath);//加载xml文件，文件

                XmlNode xns = XmlDoc.SelectSingleNode("ListBoxItem");//查找要修改的节点

                XmlNodeList ChildNode = xns.ChildNodes;//取出所有的子节点

                foreach (XmlNode xn in ChildNode)
                {
                    XmlElement Xele = (XmlElement)xn;//将节点转换一下类型
                    if (Xele.GetAttribute("id") == "-2")//判断该子节点是否是要查找的节点
                    {
                        XmlNodeList Xncn = Xele.ChildNodes;//取出该子节点下面的所有元素
                        foreach (XmlNode xn2 in Xncn)
                        {
                            XmlElement Xele2 = (XmlElement)xn2;//转换类型
                            if (Xele2.Name == "Message")//判断是否是要查找的元素
                            {
                                Xele2.InnerText = Rev;
                            }
                        }
                    }

                }
                XmlDoc.Save(XmlPath);//再一次强调 ，一定要记得保存的该XML文件
            }
            catch
            {

            }



        }


        private void ReviseJust(string Rev)
        {
            try
            {
                XmlDocument XmlDoc = new XmlDocument();
                XmlDoc.Load(XmlPath);//加载xml文件，文件

                XmlNode xns = XmlDoc.SelectSingleNode("ListBoxItem");//查找要修改的节点

                XmlNodeList ChildNode = xns.ChildNodes;//取出所有的子节点

                foreach (XmlNode xn in ChildNode)
                {
                    XmlElement Xele = (XmlElement)xn;//将节点转换一下类型
                    if (Xele.GetAttribute("id") == "-3")//判断该子节点是否是要查找的节点
                    {
                        XmlNodeList Xncn = Xele.ChildNodes;//取出该子节点下面的所有元素
                        foreach (XmlNode xn2 in Xncn)
                        {
                            XmlElement Xele2 = (XmlElement)xn2;//转换类型
                            if (Xele2.Name == "MessageJust")//判断是否是要查找的元素
                            {
                                Xele2.InnerText = Rev;
                            }
                        }
                    }

                }
                XmlDoc.Save(XmlPath);//再一次强调 ，一定要记得保存的该XML文件
            }
            catch
            {

            }



        }



        /// <summary>
        /// 弹出消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click_5(object sender, RoutedEventArgs e)
        {
            string PopMessage = "\t程序名：StickyNote\t\n";
            PopMessage += "\t完成时间：2021/3/10\t\n";
            PopMessage += "\t版本号：1.1.3"+ "\t\n";
            PopMessage += "\t代码地址：https://github.com/Xiang-M-J/StickyNote \t";
            MessageBox.Show(PopMessage);
        }
        /// <summary>
        /// 设置是否开机自启
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click_6(object sender, RoutedEventArgs e)
        {
            if (YesImage.Source == null)
            {
                YesImage.Source = new BitmapImage(new Uri("pack://application:,,,/resources/yes.png"));
                ReviseXml("true");
            }
            else
            {
                YesImage.Source = null;
                ReviseXml("false");
            }
        }

        private void MenuItem_Click_7(object sender, RoutedEventArgs e)
        {
            if (CloseImage.Source == null)
            {
                CloseImage.Source = new BitmapImage(new Uri("pack://application:,,,/resources/yes.png"));
                ReviseClose("true");
            }
            else
            {
                CloseImage.Source = null;
                ReviseClose("false");
            }
        }
    }
}