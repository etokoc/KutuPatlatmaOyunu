using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace KutuPatlatmaOyunu
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
        }

        /// <Tanımlamalar>
        Engel engelOrnek;
        Oyuncu Oyuncu = new Oyuncu();
        PictureBox ball = new PictureBox();
        PictureBox pictureBox_Kalp = new PictureBox();
        public static PictureBox tahta = new PictureBox();
        List<Engel> engelArray = new List<Engel>();
        int xhiz = 8;
        int yhiz = -2;
        Timer timer = new Timer();
        Timer timer_2 = new Timer();
        Random random = new Random();
        /// </Tanımlamalar>


        private void Form1_Load(object sender, EventArgs e)
        {
            //oyun alanı tüm formu kaplaması işlemi
            pSahne.Width = this.Width;
            pSahne.Height = this.Height;

            //ball
            ball.Image = KutuPatlatmaOyunu.Properties.Resources._58_Breakout_Tiles;
            ball.Width = 25;
            ball.Height = 25;
            ball.SizeMode = PictureBoxSizeMode.StretchImage;
            ball.Left = pSahne.Width / 2;
            ball.Top = pSahne.Height / 2;

            //tahta
            tahta.Image = KutuPatlatmaOyunu.Properties.Resources._50_Breakout_Tiles;
            tahta.SizeMode = PictureBoxSizeMode.StretchImage;
            tahta.Width = 150;
            tahta.Height = 30;
            pSahne.MouseMove += mouseMove;

            //new point kullanımı RAM'e yük bindirdiğinden dolayı 
            // tahta.Location = new Point(pSahne.Width / 2 - tahta.Width / 2, pSahne.Height - tahta.Height - 50);
            tahta.Left = pSahne.Width / 2 - tahta.Width / 2;
            tahta.Top = pSahne.Height - tahta.Height - 50;

            pictureBox_Kalp.Image = KutuPatlatmaOyunu.Properties.Resources.can_3;
            pictureBox_Kalp.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox_Kalp.BackColor = Color.Transparent;
            pictureBox_Kalp.Location = new Point(pSahne.Left+pSahne.Width-(pictureBox_Kalp.Left+pictureBox_Kalp.Width+15));

            //panele top,tahta,kalp ekleme
            pSahne.Controls.Add(tahta);
            pSahne.Controls.Add(ball);
            pSahne.Controls.Add(pictureBox_Kalp);

            //engel koy
            engelOlustur(10);
            

            //10ms de bir topu,tahtayı kontrol eder
            timer.Tick += Tick;
            timer.Interval = 10;

            //herhangi bir tuşa basarak oyunu başlatma timer'ı
            timer_2.Tick += topTahtaSabitleme;
            timer_2.Interval = 10;
            timer_2.Start();
        }

        private void topTahtaSabitleme(object sender, EventArgs e)
        {
            ball.Left = tahta.Left + (tahta.Width / 2);
            ball.Top = tahta.Top - tahta.Height;
        }

        public void mouseMove(object sender, MouseEventArgs e)
        {
            Cursor.Hide();
            tahta.Left = e.X - tahta.Width / 2;
        }

        private void Tick(object sender, EventArgs e)
        {
            //yonler 10ms de bir güncellenir
            ball.Left += xhiz;
            ball.Top += yhiz;

            //--topun  engellere vurma durumu
            for (int i = 0; i < engelArray.Count; i++)
            {
                if (engelArray[i].Left + engelArray[i].Width > ball.Left + 1 && engelArray[i].Left < ball.Left + ball.Width && engelArray[i].carpilabilmeDurumu == true)
                {
                    if (engelArray[i].Top + engelArray[i].Height > ball.Top && !(engelArray[i].Top > ball.Top + ball.Height))
                    {
                        yhiz = -yhiz;
                        xhiz = -xhiz;
                        engelArray[i].engelCan--;
                    }
                }
            }


            //topun duvarlara vurma durumu
            if (ball.Left + ball.Width > pSahne.Width || ball.Left < 1)//sağ-sol duvar
            {
                xhiz = -xhiz;
            }

            if (ball.Top < 0 || ball.Top + ball.Height > tahta.Top)//üst--alt
            {
                if (ball.Top > 0)//alt
                {
                    if (ball.Left + ball.Width > tahta.Left + tahta.Width || ball.Left + ball.Width < tahta.Left)//tahtanın yanlarından düşmesi(sağ,sol)
                    {
                        //GAME OVER
                        if (Oyuncu.oyuncuCan == 0)
                        {
                            gameOver();
                        }
                        else
                        {
                            //oyuncunun canı 0'dan büyük ise topu tahtaya yapıştırıyoruz. Bir şans daha veriyoruz
                            if (ball.Top+ball.Height>tahta.Top+tahta.Height)
                            {
                                timer.Stop();
                                if (Oyuncu.oyuncuCan==2)
                                {
                                    pictureBox_Kalp.Image = KutuPatlatmaOyunu.Properties.Resources.can_2;
                                }
                                else if (Oyuncu.oyuncuCan==1)
                                {
                                    pictureBox_Kalp.Image = KutuPatlatmaOyunu.Properties.Resources.can_1;
                                }
                                timer_2.Start();
                                Oyuncu.oyuncuCan--;
                            }
                           
                        }
                    }
                    else //tahtaya topun carpmasi
                    {
                        yhiz = -yhiz;
                    }
                }
                else //üst duvar
                {
                    yhiz = -yhiz;
                }

            }
            //engel PATLATMA bölümü
            foreach (var item in engelArray)
            {
                //engeli çatlatma kodu
                if (item.engelCan == 1)
                {
                    item.Image = KutuPatlatmaOyunu.Properties.Resources._04_Breakout_Tiles;
                }

                //engeli patlatıyorum
                if (item.engelCan == 0)
                {
                    item.Visible = false;
                    item.carpilabilmeDurumu = false;
                }
            }
        }

        public void gameOver()
        {
            timer.Stop();
            pSahne.BackColor = Color.Red;
            ball.Visible = false;
            tahta.Visible = false;
            pictureBox_Kalp.Visible = false;

            foreach (var item in engelArray)
            {
                item.Visible = false;
            }

            Label text = new Label();
            text.Text = "GAME OVER!";
            text.Font = new Font("Arial", 21, FontStyle.Bold);
            text.Left = pSahne.Left;
            text.Top = pSahne.Height / 2;
            text.Width = 300;
            text.Height = 150;
            
            pSahne.Controls.Add(text);
        }
       

        public void engelOlustur(int engelAdet)
        {
            for (int i = 0; i < engelAdet; i++)
            {
                engelOrnek = new Engel();
                engelOrnek.Left = random.Next(pSahne.Left, pSahne.Width);
                engelOrnek.Top = random.Next(pSahne.Top, pSahne.Height/2);
                engelOrnek.engelCan = 3;
                engelOrnek.carpilabilmeDurumu = true;
                engelOrnek.Image = KutuPatlatmaOyunu.Properties.Resources._03_Breakout_Tiles;
                engelOrnek.BackColor = Color.Transparent;
                engelOrnek.Width = 80;
                engelOrnek.Height = 20;

                //sahneye engeli ekledik
                pSahne.Controls.Add(engelOrnek);

                //engelleri bir diziye attım
                engelArray.Add(engelOrnek);
            }
            
        }


        //topu tahtaya yapıştırma islemi
        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            timer.Start();
            timer_2.Stop();
        }
    }
}
