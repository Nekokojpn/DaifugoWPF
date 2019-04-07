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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace DaifugoWPF
{
    /// <summary>
    /// stanim.xaml の相互作用ロジック
    /// </summary>
    public partial class stanim : Page
    {
        public stanim()
        {
            InitializeComponent();
        }
        BitmapImage[] clover = new BitmapImage[13];
        BitmapImage[] heart = new BitmapImage[13];
        BitmapImage[] spade = new BitmapImage[13];
        BitmapImage[] dia = new BitmapImage[13];
        DispatcherTimer dt = new DispatcherTimer();
        bool isVisible = false;
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            for (int i = 1; i <= 13; i++)
            {
                clover[i - 1] = new BitmapImage();
                clover[i - 1].BeginInit();
                clover[i - 1].UriSource = new Uri(System.IO.Path.GetDirectoryName(Application.ResourceAssembly.Location) + @"\tramp\clover\" + i.ToString() + ".png");
                clover[i - 1].EndInit();
            }
            for (int i = 1; i <= 13; i++)
            {
                heart[i - 1] = new BitmapImage();
                heart[i - 1].BeginInit();
                heart[i - 1].UriSource = new Uri(System.IO.Path.GetDirectoryName(Application.ResourceAssembly.Location) + @"\tramp\heart\" + i.ToString() + ".png");
                heart[i - 1].EndInit();
            }
            for (int i = 1; i <= 13; i++)
            {
                spade[i - 1] = new BitmapImage();
                spade[i - 1].BeginInit();
                spade[i - 1].UriSource = new Uri(System.IO.Path.GetDirectoryName(Application.ResourceAssembly.Location) + @"\tramp\spade\" + i.ToString() + ".png");
                spade[i - 1].EndInit();
            }
            for (int i = 1; i <= 13; i++)
            {
                dia[i - 1] = new BitmapImage();
                dia[i - 1].BeginInit();
                dia[i - 1].UriSource = new Uri(System.IO.Path.GetDirectoryName(Application.ResourceAssembly.Location) + @"\tramp\dia\" + i.ToString() + ".png");
                dia[i - 1].EndInit();
            }
            dt.Interval = TimeSpan.FromMilliseconds(700);
            dt.Tick += Dt_Tick;
            dt.Start();
        }
        Random r = new Random();
        private void Dt_Tick(object sender, EventArgs e)
        {
            
            if(isVisible==false)
            {
                dt.Interval = TimeSpan.FromMilliseconds(700);
                isVisible = true;
                img.Visibility = Visibility.Visible;
                int i = r.Next(0, 3);
                if (i == 0) img.Source = clover[r.Next(0, 12)];
                else if (i == 1) img.Source = heart[r.Next(0, 12)];
                else if (i == 2) img.Source = spade[r.Next(0, 12)];
                else if (i == 3) img.Source = dia[r.Next(0, 12)];
            }
            else
            {
                dt.Interval = TimeSpan.FromMilliseconds(300);
                isVisible = false;
                img.Visibility = Visibility.Hidden;
            }
        }
    }
}
