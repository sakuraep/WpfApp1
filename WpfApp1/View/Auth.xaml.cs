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

namespace WpfApp1.View
{

    public partial class Auth : Window
    {
        private readonly Random random = new Random();
        private readonly TradeEntities entities;
        private User user;
        private bool isRequieredcaptcha;
        private string captchaCode;
        private string captchasymbols = "QWERTYUUIOPASDFGHJKLZXCCVBNM1234567890";

        public Auth()
        {
            InitializeComponent();
            entities = new TradeEntities();
            random = new Random(Environment.TickCount);
        }
        private void OnSign(object sender, RoutedEventArgs e)
        {
            if (isRequieredcaptcha && captchaCode.ToLower() != tbCaptcha.Text.Trim())
            {
                MessageBox.Show("Неправильно введена капча");
            }
            string login = tblogin.Text.Trim();
            string password = tbpassword.Password.Trim();
            if (login.Length < 1 || password.Length < 1)
            {
                MessageBox.Show("Неверный логин или пароль");
                return;
            }
            user = entities.Users.Where(u => u.UserLogin == login && u.UserPassword == password).FirstOrDefault();
            if (user == null)
            {
                Captcha.Visibility = Visibility.Visible;
                MessageBox.Show("Некорректно введены данные");               
                generateCaptch();
                return;
            }
            if (isRequieredcaptcha)
            {
                isRequieredcaptcha = false;
            }
            switch (user.Role.RoleName)
            {
                case "Администратор":
                  AdminPanel adminPanel =  new AdminPanel(entities, user) ;
                    adminPanel.Owner = this;
                    adminPanel.Show();
                    Hide();
                    tblogin.Text = "";
                    tbpassword.Password = "";
                    tbCaptcha.Text = "";
                    canvas.Children.Clear();
                    Captcha.Visibility= Visibility.Collapsed;
                    break;
                case "Менеджер":
                    ProductView productView = new ProductView(entities, user);
                    productView.Owner = this;
                    productView.Show();
                    Hide();
                    tblogin.Text = "";
                    tbpassword.Password = "";
                    tbCaptcha.Text = "";
                    canvas.Children.Clear();
                    Captcha.Visibility = Visibility.Collapsed;
                    break;
                case "Клиент":
                    break;
            }

        }
        private void generateCaptch()
        {
            canvas.Children.Clear();
            captchaCode = newcaptchacode();

            for (int i = 0; i < captchaCode.Length; i++)
            {
                AddCharToCanvas(i, captchaCode[i]);
            }
            GenerateNoise();
        }
        private string newcaptchacode()
        {

            string code = "";
            for (int i = 0; i < 4; i++)
            {
                code += captchasymbols[random.Next(captchasymbols.Length)];
            }
            return code;

        }
        private void AddCharToCanvas(int index, char ch)
        {
            Label label = new Label();
            label.Content = ch.ToString();
            label.FontSize = random.Next(24, 30);
            label.Width = 30;
            label.Height = 60;
            label.Foreground = GetRandomColor();   
            label.FontWeight= FontWeights.Black;
            label.HorizontalAlignment = HorizontalAlignment.Center;
            label.VerticalContentAlignment = VerticalAlignment.Center;
            label.RenderTransformOrigin = new Point(0.5, 0.5);
            label.RenderTransform = new RotateTransform(random.Next(-20, 15));
            canvas.Children.Add(label);
            int StartPosition = (int)((canvas.ActualWidth / 2) - (30 * 4 / 2));
            Canvas.SetLeft(label, StartPosition + (index * 30));
            Canvas.SetTop(label, random.Next(0, 10));

        }
        private SolidColorBrush GetRandomColor()
        {
            return new SolidColorBrush(Color.FromRgb((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256)));

        }
        private void GenerateNoise()
        {
            for (int i = 1; i < 150; i++)
            {
                // Не знаю какая высота и ширина, по этому так
                double x = random.NextDouble() * 290;
                double y = random.NextDouble() * 60;
                int radius = random.Next(5, 8);
                Ellipse ellipse = new Ellipse
                {
                    Width = radius,
                    Height = radius,
                    Fill = GetRandomColor(),
                    Stroke = Brushes.Transparent
                };
                canvas.Children.Add(ellipse);

                Canvas.SetLeft(ellipse, x);
                Canvas.SetTop(ellipse, y);
                
            }
            canvas.Background = new SolidColorBrush(Color.FromRgb(245, 242, 235));
            int linecount = random.Next(2, 5);
            for(int i = 0; i < linecount; i++)
            {
                Line line = new Line();
                line.X1 = random.Next(20, 120);
                line.Y1 = random.Next(10, 54);
                line.X2 = random.Next(260, 300);
                line.Y2 = random.Next(10, 54);
                line.Stroke = GetRandomColor();
                line.StrokeThickness=random.Next(2,4);
                
                canvas.Children.Add(line);
            }

            
        }
    }
}
