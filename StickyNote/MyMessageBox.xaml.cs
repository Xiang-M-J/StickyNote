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
    /// MyMessageBox.xaml 的交互逻辑
    /// 该xmal暂时没有实际用处
    /// </summary>
    public partial class MyMessageBox : Window
    {
        public string GetStr { get; set; }
        public MyMessageBox()
        {
            InitializeComponent();
            try
            {
                MessageBox.Show(GetStr);
                string[] TimeArray = GetStr.Split(' ');
                textBlock2.Text = TimeArray[0];
                textBlock3.Text = TimeArray[1];
            }
            catch
            {
                MessageBox.Show("Some errors happen");
            }
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

    }
}
