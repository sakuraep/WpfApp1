using System;
using System.Collections.Generic;
using System.Data.Entity;
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
    /// <summary>
    /// Логика взаимодействия для AdminPanel.xaml
    /// </summary>
    public partial class AdminPanel : Window
    {
        private readonly TradeEntities entities;
        private readonly User user;
        public AdminPanel(TradeEntities entities, User user)
        {
            this.entities = entities;
            this.user = user;
            InitializeComponent();
            
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            if (Owner != null)
            {
                Owner.Show();
            }
        }

        private void ViewButton_Click(object sender, RoutedEventArgs e)
        {
            ProductView productView = new ProductView(entities, user);
            productView.Owner = this;
            productView.Show();
            Hide();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AddProduct addproduct = new AddProduct(entities, user);
            addproduct.Owner = this;
            addproduct.Show();
            addproduct.isEdit = false;
            Hide();
        }
    }
}
