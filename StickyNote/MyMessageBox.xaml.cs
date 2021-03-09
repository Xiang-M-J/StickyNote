using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace StickyNote
{
    /// <summary>
    /// MyMessageBox.xaml 的交互逻辑
    /// 该xmal暂时没有实际用处
    /// </summary>
    public partial class MyMessageBox : Window
    {
        public delegate void SendMessage(string[] InfoT);
        public SendMessage sendMessage;
        //public string GetStr { get; set; }
        public MyMessageBox(double top, double left)
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.Manual;
            this.Top = top;
            this.Left = left;
        }
        private void Image3_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }

        }
        private void TB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex re = new Regex("[^0-9.-]+");
            e.Handled = re.IsMatch(e.Text);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string[] StInfo = new string[2];
            StInfo[0] = NewInfo.Text;
            string YearM = " ";
            try
            {
                YearM = Convert.ToDateTime(NewDate.Text).ToString("yyyy/MM/dd");
            }
            catch
            {
                YearM = DateTime.Now.ToString("yyyy/MM/dd");
            }
            
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
            StInfo[1] = TimeT;

            if (StInfo[0].Length != 0)
            {
                sendMessage(StInfo);
            }
            
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            NewInfo.Text = null;
            NewDate.Text = null;
            hour.Text = null;
            minute.Text = null;
            this.Close();
        }
    }
}
