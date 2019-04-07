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
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.IO;
using System.Windows.Media.Animation;

namespace DaifugoWPF
{
    /// <summary>
    /// game.xaml の相互作用ロジック
    /// </summary>
    public partial class game : Window
    {
        public game()
        {
            InitializeComponent();
        }
        #region デバッグ用メッセージ
        void a(string debtext)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Write(debtext + "\r\n");
        }
        void a_ly()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Write("*" + "\r\n");
        }
        void a_event(string debtext)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.DarkGreen;
            Console.Write(debtext + "\r\n");
            Console.BackgroundColor = ConsoleColor.Black;
        }
    
        void a_method(string debtext)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.DarkCyan;
            Console.Write(debtext + "\r\n");
            Console.BackgroundColor = ConsoleColor.Black;
        }
        void a_script(string debtext)
        {
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.Yellow;
            Console.Write(debtext + "\r\n");
        }
        void a_error(string debtext)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Red;
            Console.Write(debtext + "\r\n");
        }
        #endregion
        public void status_update(string msg)
        {
            this.Dispatcher.Invoke(() =>
            {
                status.Text = msg;
            });
        }
        public async void sendMes_ori(string mes)
        {
            byte[] sendBytes = enc.GetBytes(mes + "\n");
            await ns.WriteAsync(sendBytes, 0, sendBytes.Length);
        }
        public async void sendMes(string mes)
        {
            Console.WriteLine("メッセージ:[" + name + "]" + "{" + mes + "}" + "\n");
            byte[] sendBytes = enc.GetBytes("name:" + name + ";endname" + "message{" + mes + "}endmessage" + "\n");
            await ns.WriteAsync(sendBytes, 0, sendBytes.Length);
        }
        public async Task<string> resGetAsync()
        {
            ms.SetLength(0);
            //データの一部を受信する
            resSize = await ns.ReadAsync(resBytes, 0, resBytes.Length);
            //受信したデータを蓄積する
            await ms.WriteAsync(resBytes, 0, resSize);
            string resMsg = enc.GetString(ms.GetBuffer(), 0, (int)ms.Length);
            Console.WriteLine("受信:["+resMsg+ "\n");
            return resMsg;
        }
        public string name;
        public string ipaddress;
        string placestring = "";
        List<string> selectedcard = new List<string>();
        string mycard;
        string[] namelist = new string[4];
        byte[] resBytes = new byte[1024];
        System.Text.Encoding enc = System.Text.Encoding.Unicode;
        int resSize = 0;
        System.Net.Sockets.NetworkStream ns;
        System.IO.MemoryStream ms = new System.IO.MemoryStream();
        int[] mycardV = new int[14];
        string[] mycardK = new string[14];
        int mycardR = 0;
        bool isSendPacket = false;

        bool isreq = false;
        bool ischat = false;


        BitmapImage[] clover = new BitmapImage[13];
        BitmapImage[] heart = new BitmapImage[13];
        BitmapImage[] spade = new BitmapImage[13];
        BitmapImage[] dia = new BitmapImage[13];
        BitmapImage joker = new BitmapImage();
        BitmapImage back = new BitmapImage();


        stanim sa = new stanim();
        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            f.Navigate(sa);
            TcpClient tcp = new TcpClient(ipaddress, 2001);
            Console.WriteLine("サーバー({0}:{1})と接続しました({2}:{3})。",
            ((System.Net.IPEndPoint)tcp.Client.RemoteEndPoint).Address,
            ((System.Net.IPEndPoint)tcp.Client.RemoteEndPoint).Port,
            ((System.Net.IPEndPoint)tcp.Client.LocalEndPoint).Address,
            ((System.Net.IPEndPoint)tcp.Client.LocalEndPoint).Port);
            ns = tcp.GetStream();


            //send name
            byte[] sendBytes = enc.GetBytes(name + '\n');
            ns.Write(sendBytes, 0, sendBytes.Length);

            //サーバーから送られたデータを受信する


            await Task.Run(async () =>
            {

                while (true)
                {
                    string resMsg = await resGetAsync();
                    status_update(resMsg);
                    if (resMsg.IndexOf("全員") > -1) break;
                    await Task.Delay(100);
                }
                //ゲームスタート!!
                //Get namelist
                while (true)
                {
                    string temp = await resGetAsync();
                    if (temp.IndexOf("namelist[") > -1)
                    {
                        Console.WriteLine(temp);
                        int i = 0;
                        MatchCollection mc = Regex.Matches(temp, @"\{(.+?)\}", RegexOptions.Singleline);
                        foreach (Match m in mc)
                        {
                            string tem = m.ToString().Replace("{", "").Replace("}", "").Replace("\r\n", "").Replace("\n", "");
                            namelist[i++] = tem;
                        }
                        break;
                    }
                    await Task.Delay(100);
                }
                int cnt = 0;
                foreach (string s in namelist)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        if (s != name)
                        {
                            if (cnt == 0)
                            {
                                name1.Name = s;
                                name1.Text = s+"[0]";
                            }
                            else if (cnt == 1)
                            {
                                name2.Name = s;
                                name2.Text = s + "[0]";
                            }
                            else
                            {
                                name3.Name = s;
                                name3.Text = s + "[0]";
                            }
                            cnt++;
                        }
                        else
                        {
                            name_me.Name = name;
                            name_me.Text = name + "[0]";
                        }
                    });
                }
                await this.Dispatcher.Invoke(async () =>
                 {
                     sa.status.Text = "読み込んでいます";

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
                     joker.BeginInit();
                     joker.UriSource = new Uri(System.IO.Path.GetDirectoryName(Application.ResourceAssembly.Location) + @"\tramp\joker.png");
                     joker.EndInit();
                     back.BeginInit();
                     back.UriSource = new Uri(System.IO.Path.GetDirectoryName(Application.ResourceAssembly.Location) + @"\tramp\back.jpg");
                     back.EndInit();
                     await Task.Delay(1500);
                     sa.status.Text = "ゲーム開始を待機しています";
                     sendMes("OKREADY");
                     //ゲーム開始を待機する
                     string temp = await resGetAsync();
                     while (true)
                     {
                         
                         if (temp.IndexOf("gamestart") > -1) break;
                         await Task.Delay(500);
                     }
                     //カードを初期化
                     for(int i = 0; i<14;i++)
                     {
                         mycardV[i] = -1;
                     }
                     //カード情報をゲット
                     while (true)
                     {
                         
                         if (temp.IndexOf("cardinfo") > -1)
                         {
                             mycard = temp;
                             MatchCollection kmc = Regex.Matches(temp, @"\[(.+?)\]");
                             MatchCollection vmc = Regex.Matches(temp, @"\{(.+?)\}");
                             int mCnt = 0;
                             int mmCnt = 0;
                             foreach (Match m in kmc)
                             {
                                 foreach (Match mm in vmc)
                                 {
                                     if (mCnt == mmCnt)
                                     {
                                         string ms = m.ToString().Replace("[", "").Replace("]", "");
                                         string mms = mm.ToString().Replace("{", "").Replace("}", "");
                                         Console.WriteLine(ms + mms);
                                         mycardK[mCnt] = ms;
                                         mycardV[mCnt] = int.Parse(mms);
                                     }
                                     mmCnt++;
                                 }

                                 mmCnt = 0;
                                 mCnt++;
                             }
                             mycardR = mCnt;
                             break;
                         }
                         
                         await Task.Delay(500);
                         temp = await resGetAsync();
                     }

                     {
                         Image img = new Image();
                         img.Source = back;
                         img.Width = 28;
                         img.Height = 100;
                         sp1.Children.Add(img);
                     }

                     {
                         Image img = new Image();
                         img.Source = back;
                         img.Width = 28;
                         img.Height = 100;
                         sp2.Children.Add(img);
                     }

                     {
                         Image img = new Image();
                         img.Source = back;
                         img.Width = 28;
                         img.Height = 100;
                         sp3.Children.Add(img);
                     }
                     status.Text = "";
                 });
                while (true)
                {
                    updateCard();
                    await Task.Delay(1000);//カードを更新してゲームを進行していきます
                }

            });
            //閉じる
            ns.Close();
            tcp.Close();
            Console.WriteLine("切断しました。");

        }

        async void updateCard()
        {
            int cntc = 0;//残りカード数
            this.Dispatcher.Invoke(() =>
            {


                mysp.Children.Clear();
                try
                {
                    

                    for (int i = 0; i < 14; i++)
                    {
                        if (mycardV[i] != -1)
                        {
                            Image img = new Image();
                            img.BeginInit();
                            img.Width = 90;
                            img.Height = 180;
                            if (mycardK[i] == "C")
                            {

                                img.Source = clover[mycardV[i] - 1];
                                img.EndInit();
                                mysp.Children.Add(img);
                            }
                            else if (mycardK[i] == "S")
                            {
                                img.Source = spade[mycardV[i] - 1];
                                img.EndInit();
                                mysp.Children.Add(img);
                            }
                            else if (mycardK[i] == "H")
                            {
                                img.Source = heart[mycardV[i] - 1];
                                img.EndInit();
                                mysp.Children.Add(img);
                            }
                            else if (mycardK[i] == "D")
                            {
                                img.Source = dia[mycardV[i] - 1];
                                img.EndInit();
                                mysp.Children.Add(img);
                            }
                            else if (mycardK[i] == "J")
                            {
                                img.Source = joker;
                                img.EndInit();
                                mysp.Children.Add(img);
                            }
                            img.Name = "n" + mycardK[i] + "nv" + mycardV[i].ToString() + "vnum" + i + "num";
                            img.MouseLeftButtonDown += (sen, ee) =>
                              {
                                  string m = Regex.Match(img.Name, @"n(.+?)n").ToString().Replace("n", "");
                                  string mm = Regex.Match(img.Name, @"v(.+?)v").ToString().Replace("v", "");
                                  string mn = Regex.Match(img.Name, @"num(.+?)num").ToString().Replace("num", "");
                              //Not selected
                                  selectedcard.Remove("[" + m + "]{" + mm + "}(" + mn + ")");
                                  selectedcard.Add("[" + m + "]{" + mm + "}(" + mn + ")");



                                  string ss = "提出:\r\n";
                                  foreach (string s in selectedcard)
                                  {
                                      ss += s;
                                  }
                                  teisyutu.Text = ss;
                              };
                        }
                    }
                }
                catch (Exception) { }
            });
            //count card remaining
            for(int i = 0;i<14;i++)
            {
                if (mycardV[i] != -1) cntc++;
            }
            //sendMessageを構築します
            string sendmessage = $"remain:{cntc};endremain";
            
            if (!isSendPacket)
            {
                if (isreq)
            {
                isreq = false;
                sendmessage += reqstring;
            }
            if(ischat)
            {
                ischat = false;
                sendmessage += sendchat;
            }
                isSendPacket = true;
                Console.WriteLine("sendmessage:到達");
                sendMes("OKpacket:" + sendmessage);
                this.Dispatcher.Invoke(() =>
                {
                    f.Visibility = Visibility.Hidden;
                });
                string temp;
                /*
                //TCP- OKが出るまで待機
                while (true)
                {
                    //Interval
                    temp = await resGetAsync();
                    if (temp.IndexOf("OK") > -1) break;
                    await Task.Delay(250);
                }
                */
                //こちらもOKメッセージを送信
                //packetデータを待機する
                while (true)
                {
                    temp = await resGetAsync();
                    Console.WriteLine($"packetを待機しています");
                    if (temp.IndexOf("packet:") > -1) break;
                    await Task.Delay(100);
                }
                //Console.WriteLine($"packet受信{temp}");
                isSendPacket = false;
               
                MatchCollection mc = Regex.Matches(temp, "name:(.+?)endmessage");
                try
                {
                    foreach (Match m in mc)
                    {
                        a_method(m.ToString());
                        //remainをupdate
                        string m_name = Regex.Match(m.ToString(), "name:(.+?)endname").ToString().Replace("name:", "").Replace(";endname", "");
                        string m_remain = Regex.Match(m.ToString(), "remain:(.+?)endremain").ToString().Replace("remain:", "").Replace(";endremain", "");
                        try
                        {
                            this.Dispatcher.Invoke(() =>
                            {
                                if (m_name == name1.Name)
                                {
                                    name1.Text = name1.Text.Replace(Regex.Match(name1.Text, @"\[(.+?)\]").ToString(), "");
                                    name1.Text += $"[{m_remain}]";
                                }
                                else if (m_name == name2.Name)
                                {
                                    name2.Text = name2.Text.Replace(Regex.Match(name2.Text, @"\[(.+?)\]").ToString(), "");
                                    name2.Text += $"[{m_remain}]";
                                }
                                else if (m_name == name3.Name)
                                {
                                    name3.Text = name3.Text.Replace(Regex.Match(name3.Text, @"\[(.+?)\]").ToString(), "");
                                    name3.Text += $"[{m_remain}]";
                                }
                                else
                                {
                                    name_me.Text = name_me.Text.Replace(Regex.Match(name_me.Text, @"\[(.+?)\]").ToString(), "");
                                    name_me.Text += $"[{m_remain}]";
                                }
                            });
                        }
                        catch (Exception) { }
                        Match m_chat = Regex.Match(m.ToString(), "Chat:(.+?)endChat");
                        if (m_chat.Success)
                        {
                            string m_chatstr = m_chat.ToString().Replace("Chat:", "").Replace(";endChat", "");
                            this.Dispatcher.Invoke(() =>
                            {
                                chatText.Text += "[" + m_name + "]" + m_chatstr.ToString() + Environment.NewLine;
                            });
                        }
                        Match mr = Regex.Match(m.ToString(), "Req:(.+?)endReq");

                        if (mr.Success)
                        {
                            string req = mr.ToString().Replace("Req:", "").Replace("endReq", "");
                            a_event($"提出リスエストがありました{req}");
                            //Storyboard stb = FindResource("upd") as Storyboard;
                            //stb.Begin();
                            
                            MatchCollection mck = Regex.Matches(req, @"\[(.+?)\]");
                            MatchCollection mcv = Regex.Matches(req, @"\{(.+?)\}");
                            this.Dispatcher.Invoke(() =>
                            {
                                tsp.Children.Clear();
                                int cnt = 0;
                                foreach (Match mckc in mck)
                                {
                                    int ccnt = 0;
                                    string te = mckc.ToString();
                                    foreach (Match mcvc in mcv)
                                    {
                                        if (ccnt == cnt)
                                        {
                                            int i = int.Parse(mcvc.ToString().Replace("{", "").Replace("}", ""));
                                            Image img = new Image();
                                            img.BeginInit();
                                            img.Width = 90;
                                            img.Height = 180;
                                            if (te.IndexOf("S") > -1)
                                            {
                                                img.Source = spade[i - 1];
                                            }
                                            else if (te.IndexOf("C") > -1)
                                            {
                                                img.Source = clover[i - 1];
                                            }
                                            else if (te.IndexOf("D") > -1)
                                            {
                                                img.Source = dia[i - 1];
                                            }
                                            else if (te.IndexOf("H") > -1)
                                            {
                                                img.Source = heart[i - 1];
                                            }
                                            else
                                            {
                                                img.Source = joker;
                                            }
                                            img.EndInit();
                                            tsp.Children.Add(img);
                                            break;
                                        }
                                        ccnt++;
                                    }
                                    cnt++;
                                }
                            });
                        }
                    }
                }
                catch (Exception) { }
            }
        }


        string reqstring = "";
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            reqstring = "Req:";
            foreach(string s in selectedcard)
            {
                reqstring += s;

            }
            reqstring+= ";endReq";
            if (reqstring != "Req:")
            {
                teisyutu.Text = "提出:";
                //Removes selected card(s)
                foreach(string s in selectedcard)
                {
                    int ccnt = int.Parse(Regex.Match(s, @"\((.+?)\)").ToString().Replace("(","").Replace(")",""));
                    mycardK[ccnt] = "N";
                    mycardV[ccnt] = -1;
                }
                selectedcard.Clear();
                isreq = true;
            }

        }
        string sendchat = "";
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            sendchat = "Chat:";
            if (!string.IsNullOrEmpty(chatinp.Text))
            {
                sendchat += chatinp.Text + ";endChat";
            }
            if (sendchat != "Chat:")
            {
                chatinp.Text = "";
                ischat = true;
            }
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            selectedcard.Clear();
        }
    }
}
