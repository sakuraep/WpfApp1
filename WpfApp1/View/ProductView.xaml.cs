using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.ComponentModel;
using Microsoft.Win32;
using System.IO;
using System.IO.Pipes;
using System.Security.Cryptography;
using System.Xml.Linq;

namespace WpfApp1.View
{
   public class SortItem
    {
        public string Text { get; set; }
        public SortDescription Description { get; set; }
    }

    public partial class ProductView : Window
    {
        private readonly TradeEntities entities;
        public ObservableCollection<Product> Products { get; set; }
        public ObservableCollection<Manafacturer> Manafacts { get; set; }
        public ObservableCollection<SortItem> SortItems { get; set; }
        private readonly Manafacturer allmanufacturers;
        public SortItem SelectedSort {  get; set; }
        public Stream filestream { get; set; }
        public Manafacturer SelectedManufacturer { get; set; }
        public Product Selectedproduct { get; set; }
        public User activeuser { get; set; }
        public int userRole { get; set; }
        
        public ProductView(TradeEntities entities, User user)

        {
            InitializeComponent();
            this.entities = entities;
            AddProduct addProduct = new AddProduct(entities, activeuser);
            Products = new ObservableCollection<Product>(entities.Products);
            Manafacts = new ObservableCollection<Manafacturer>(entities.Manafacturers);
            allmanufacturers = new Manafacturer() { ID = 0, Name = "Все производители" };
            Manafacts.Insert(0, allmanufacturers);
            activeuser = user;
            SortItems = new ObservableCollection<SortItem>()
            {
                new SortItem() { Text = "Сортировка по убыванию", Description = new SortDescription() { PropertyName = "ProductCost", Direction = ListSortDirection.Descending } },
                new SortItem() { Text = "Сортировка по возрастанию", Description = new SortDescription() { PropertyName = "ProductCost", Direction = ListSortDirection.Ascending } }
            };

            userRole = user.UserRole;
            DataContext = this;
            
            //Binding category = new Binding();
            //category.ElementName = "Selectedproduct";
            //category.Path = new PropertyPath("Selectedproduct.ProductCategory");
            //addProduct.cbCategory.SetBinding(Selectedproduct.)

        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if(Owner != null)
            {
                Owner.Show();
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        { 
            ApplyFilter();
        }
        private void ApplyFilter()
        {
            
            var searchstring = tbsearch.Text.Trim().ToLower();
            
           var view = CollectionViewSource.GetDefaultView(lvProducts.ItemsSource);
            if(view == null) return;
            view.Filter = (object o) =>
            {
                var product = o as Product;
                if (product == null) return true;
                if (searchstring.Length > 0)
                {
                    if (!(product.ProductName.ToLower().Contains(searchstring) ||
                     product.ProductDescription.ToLower().Contains(searchstring) ||
                     product.ProductCategory1.Name.ToLower().Contains(searchstring) ||
                     product.Provider.Name.ToLower().Contains(searchstring) ||
                     product.Manafacturer.Name.ToLower().Contains(searchstring)))
                    {
                        return false;
                    }
                }
                if (SelectedManufacturer != null && SelectedManufacturer != allmanufacturers)
                {
                    if(product.Manafacturer != SelectedManufacturer)
                    {
                        return false;
                    }
                }       
                        return true;
            };
            
        }

        private void ApplySort()
        { 
            var view = CollectionViewSource.GetDefaultView(lvProducts.ItemsSource);
            if (view == null) return;
            view.SortDescriptions.Clear();
            if(SelectedSort != null)
            {
                view.SortDescriptions.Add(SelectedSort.Description);
            }
            
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void ComboBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            ApplySort();
        }
       

        private void lvProducts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            
            if (userRole == 1)
            {
                
                
                AddProduct addProduct = new AddProduct(entities, activeuser);

                addProduct.setItem(Selectedproduct);
                addProduct.Show();
                addProduct.isEdit = true;
                
              
            }
            else return;


            //filestream.Read(Selectedproduct.ProductPhoto,0,Selectedproduct.ProductPhoto.Length);
            //filestream.Seek(0, SeekOrigin.Begin);
            //BitmapImage bitmap = new BitmapImage();
            //bitmap.BeginInit();
            //bitmap.StreamSource = filestream;
            //bitmap.EndInit();
            //addProduct.ImagePreview.Source = bitmap;


        }
    }
}
