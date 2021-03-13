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

namespace StickyNote
{
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class CloseMessage : Window
    {
        public delegate void SendMessage(string[] CloseMeg);
        public SendMessage sendMessage;
        public CloseMessage()
        {
            InitializeComponent();
        }
        #region 自定义窗口

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
        #endregion

        

        /// <summary>
        /// 向主窗口发送信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string[] CloseMg = new string[2] { "Mini","false"};
            if (MiniButton.IsChecked == true)
            {
                CloseMg[0] = "Mini";
            }
            else
            {
                CloseMg[0] = "Exit";
            }
            if (checkBox.IsChecked==true)
            {
                CloseMg[1] = "true";
            }
            sendMessage(CloseMg);
            this.Close();
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
