using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.IO;

namespace WpfApp1.View
{
  
    public partial class AddProduct : Window
    {
        private Product currentProduct = new Product();
        private readonly TradeEntities entities;
        public ObservableCollection<Manafacturer> Manafacts { get; set; }
        public ObservableCollection<Provider> Providers { get; set; }
        public ObservableCollection<Unit> Units { get; set; }
        public ObservableCollection<ProductCategory> ProductCategories { get; set; }
        public byte[] photo { get; set; }
        public bool isEdit { get; set; }
        public AddProduct(TradeEntities entities, User user)
        {
            InitializeComponent();
            this.entities = entities;

            Manafacts = new ObservableCollection<Manafacturer>(entities.Manafacturers);
            Providers = new ObservableCollection<Provider>(entities.Providers);
            Units = new ObservableCollection<Unit>(entities.Units);
            ProductCategories = new ObservableCollection<ProductCategory>(entities.ProductCategories);
            DataContext = this;
            cbCategory.SetBinding(ItemsControl.ItemsSourceProperty, new Binding()
            {
                Source = ProductCategories
            });
            cbManufacturer.SetBinding(ItemsControl.ItemsSourceProperty, new Binding()
            {
                Source = Manafacts
            });
            cbProvider.SetBinding(ItemsControl.ItemsSourceProperty, new Binding()
            {
                Source = Providers
            });
            cbUnit.SetBinding(ItemsControl.ItemsSourceProperty, new Binding()
            {
                Source = Units
            });
        }

        private void AddImage_Click(object sender, RoutedEventArgs e)
        {
            Product product = new Product();
            if (product == null) return;
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Файлы изображений|*.jpg;*.jpeg;*.png;";
            fileDialog.Multiselect = false;
            if(fileDialog.ShowDialog()== true)
            {
                Stream fileStream = fileDialog.OpenFile();
                product.ProductPhoto = new byte[fileStream.Length];
                fileStream.Read(product.ProductPhoto,0,(int)fileStream.Length);
                fileStream.Seek(0, SeekOrigin.Begin);
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = fileStream;
                bitmap.EndInit();
                ImagePreview.Source = bitmap;
                if(currentProduct.ProductPhoto == null)
                {
                    photo = product.ProductPhoto;
                }
                else { photo = null; }
                
            }
        }
        public void setItem(Product product)
        {
            DataContext = product;
        }
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (isEdit)
            {

                
                entities.SaveChanges();
                
            }
            else
            {
                var category = entities.ProductCategories.Where(c => c.Name.Equals(cbCategory.Text)).FirstOrDefault();
                var manufacturer = entities.Manafacturers.Where(d => d.Name.Equals(cbManufacturer.Text)).FirstOrDefault();
                var unit = entities.Units.Where(f => f.Name.Equals(cbUnit.Text)).FirstOrDefault();
                var provider = entities.Providers.Where(g => g.Name.Equals(cbProvider.Text)).FirstOrDefault();
                currentProduct.ProductManufacturer = manufacturer.ID;
                currentProduct.ProductName = tbName.Text;
                currentProduct.ProductDescription = tbDescription.Text;
                currentProduct.ProductCategory = category.ID;
                currentProduct.ProductProvider = provider.ID;
                currentProduct.ProductCost = int.Parse(tbCost.Text);
                currentProduct.ProductDiscountAmount = (byte)int.Parse(tbCurrentAmount.Text);
                currentProduct.ProductMaxDiscount = (byte)int.Parse(tbMaxAmount.Text);
                currentProduct.ProductQuantityInStock = int.Parse(tbQuantity.Text);
                currentProduct.ProductArticleNumber = tbArticle.Text;
                currentProduct.Unit = unit.ID;
                currentProduct.ProductPhoto = photo;


                entities.Products.Add(currentProduct);
                entities.SaveChanges();
                tbArticle.Clear();
                tbCurrentAmount.Clear();
                tbQuantity.Clear();
                tbDescription.Clear();
                tbMaxAmount.Clear();
                tbName.Clear();
                tbCost.Clear();
                cbCategory.SelectedIndex = 0;
                cbManufacturer.SelectedIndex = 0;
                cbProvider.SelectedIndex = 0;
                cbUnit.SelectedIndex = 0;
                ImagePreview = null;
            }

          
            
        }
        


        private void Window_Closed(object sender, EventArgs e)
        {
            if (Owner != null)
            {
                Owner.Show();
                
            }
        }
    }
}
