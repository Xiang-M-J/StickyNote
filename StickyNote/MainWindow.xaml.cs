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

namespace StickyNote
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public string XmlPath = System.IO.Directory.GetCurrentDirectory() + "/configure.xml";  // 定义Xml文件路径，使其与exe文件处于同一目录下
        public MainWindow()
        {
            InitializeComponent();

            StartAutomaticallyCreate("StickyNote");
            //StartAutomaticallyDel("StickyNote");
            Dater.Text = DateTime.Today.ToString();  // 日期选择框初始化
            ReadFile();  // 读取文件初始化
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
            this.Close();
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
            gs1.Color = Colors.Red;
            brush.GradientStops.Add(gs1);

            GradientStop gs2 = new GradientStop();
            gs2.Offset = 0.5;
            gs2.Color = Colors.Blue;
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
            writer.WriteStartElement("item");
            writer.WriteAttributeString("id", "-1");
            MessageBox.Show(str);
            writer.WriteElementString("autostart",str);
            writer.Close();
        }

        /// <summary>
        /// 读取Xml文件
        /// </summary>
        private void ReadFile()
        {
            List<Item> TempItemList = new List<Item>();
            XmlDocument Reader = new XmlDocument();
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

            FileFlash();
        }

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
            Regex re = new Regex("[^0-9.-]+");
            e.Handled = re.IsMatch(e.Text);
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
                    Item newitem = (Item)MyListBox.SelectedItem;

                    //if (newitem.Time != "每天")
                    //{

                    //    MessageBox.Show(newitem.Time);
                    //}
                    //else
                    //{
                    //    MessageBox.Show("每天");
                    //}

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
                    plaintext += (i + 1).ToString() + "\t" + ItemList[i].Info + "\t" + ItemList[i].Time + "\n";
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
                ".markdown-body table{margin-top: 0;margin-bottom: 16px;}.markdown-body hr{height: 4px;padding: 0; margin: 16px 0; background-color: #e7e7e7;border: 0 none;}\n" +
                ".markdown-body table{display: block;width: 100%;overflow: auto;word-break: normal; word-break: keep-all;}.markdown-body table th {font-weight: bold;}\n" +
                ".markdown-body table th,.markdown-body table td {padding: 6px 13px;border: 1px solid #ddd;}.markdown-body table tr { background-color: #fff;border-top: 1px solid #ccc;}\n" +
                ".markdown-body table tr:nth-child(2n){ background-color: #f8f8f8;}.markdown-body code{ padding: 0; padding-top: 0.2em; padding-bottom: 0.2em; margin: 0;font-size: 100%;background-color: rgba(0,0,0,0.04);border-radius: 3px;}\n" +
                ".markdown-body code:before,.markdown-body code:after { letter-spacing: -0.2em; content: \" \\00a0\";}</style>";
                htmltext += "<title>便笺</title></head>\n <body>\n";
                htmltext += " <article class=\"markdown - body\"><h4>本文件于<code>" + DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss") + "</code>创建</h4>";
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
                MessageBox.Show("Error");
            }
              return false;
        }
        private string ReadXml()
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(XmlPath);
                XmlNodeList lis = doc.GetElementsByTagName("autostart");
                string str = lis[lis.Count-1].InnerText;
                return str;
            }
            catch
            {
                MessageBox.Show("Error");
                return "null";
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
    }
}