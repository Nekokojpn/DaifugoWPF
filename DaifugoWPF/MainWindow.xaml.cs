using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DaifugoWPF
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(name_text.Text)&& !string.IsNullOrWhiteSpace(ip_text.Text))
            {
                game g = new game();
                g.name = name_text.Text;
                g.ipaddress = ip_text.Text;
                g.Show();
                this.Hide();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Read name and ip
            using (StreamReader sr = new StreamReader("ip.txt"))
            {
                ip_text.Text = sr.ReadToEnd();

            }
            using (StreamReader sr = new StreamReader("name.txt"))
            {
                name_text.Text = sr.ReadToEnd();
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                if (!string.IsNullOrWhiteSpace(name_text.Text) && !string.IsNullOrWhiteSpace(ip_text.Text))
                {
                    game g = new game();
                    g.name = name_text.Text;
                    g.ipaddress = ip_text.Text;
                    g.Show();
                    this.Hide();
                }
            }
        }
    }
}
