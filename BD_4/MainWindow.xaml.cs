using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
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

namespace BD_4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SqlConnection con;
        DataSet ds;
        public MainWindow()
        {
            InitializeComponent();
            con = new SqlConnection(ConfigurationManager.ConnectionStrings["Connect1"].ConnectionString);
            ds = new DataSet();
        }

        private void Table_Selected(object sender, RoutedEventArgs e)
        {
            SqlCommand s = new SqlCommand();
            TreeViewItem t = (TreeViewItem)sender;
            t.Items.Clear();
            if (t.Header.ToString() == "Таблиці")
            {
                s.CommandText = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES where Not(TABLE_NAME like 'sysdiagrams')";
                s.Connection=con;
                }
                if (t.Header.ToString() == "Процедури")
                {
                s.CommandText = "SELECT name FROM sys.objects o WHERE o.[Type] = 'P' and not(name like 'sp%')";
                s.Connection=con;
            }
            con.Open();
            SqlDataReader R = s.ExecuteReader();
            if ((R.HasRows) & (t.Header.ToString() == "Таблиці"))
            {
                while (R.Read())
                {
                    TreeViewItem ts = new TreeViewItem();
                    ts.Header = R[0].ToString();
                    ts.Selected += Table_Selected1;
                    t.Items.Add(ts);
                }
            }
            if ((R.HasRows) & (t.Header.ToString() == "Процедури"))
            {
                while (R.Read())
                {
                    TreeViewItem ts = new TreeViewItem();
                    ts.Header = R[0].ToString();
                    ts.Selected += Proc_Selected;
                    t.Items.Add(ts);
                }
            }
            con.Close();
        }

        private void Table_Selected1(object sender, RoutedEventArgs e)
        {
            TreeViewItem t = (TreeViewItem)sender;
            SqlCommand s = new SqlCommand("SELECT * from " + t.Header.ToString(), con);
            con.Open();
            SqlDataAdapter DA = new SqlDataAdapter(s);
            ds.Clear(); /////
            DA.Fill(ds, t.Header.ToString());
            DataGrid d = new DataGrid();

            d.AutoGenerateColumns = true;
            d.ItemsSource = ds.Tables[t.Header.ToString()].DefaultView;
            ContextMenu c = new ContextMenu();
            MenuItem m = new MenuItem();
            m.Header = "Закрити";
            m.Click += MenuItem_Click;
            c.Items.Add(m);
            tabs.Items.Add(
                new TabItem
                    {
                        Header = t.Header.ToString(),
                        Content = d,
                        ContextMenu = c
                    });
            con.Close();
            Status.Text = "Завантажено таблицю " + t.Header.ToString();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            tabs.Items.RemoveAt(tabs.SelectedIndex);
        }

        private void Proc_Selected(object sender, RoutedEventArgs e)
        {
            TreeViewItem t = (TreeViewItem)sender;
            SqlCommand s = new SqlCommand();
            s.CommandType = CommandType.StoredProcedure;
            s.CommandText = "proc_selectATC";
            s.Connection = con;
            con.Open();
            SqlDataAdapter DA = new SqlDataAdapter(s);
            ds.Clear(); /////
            DA.Fill(ds, "proc_selectATC");
            DataGrid d = new DataGrid();
            d.AutoGenerateColumns = true;
            d.ItemsSource = ds.Tables["proc_selectATC"].DefaultView;
            ContextMenu c = new ContextMenu();
            MenuItem m = new MenuItem();
            m.Header = "Закрити";
            m.Click += MenuItem_Click;
            c.Items.Add(m);
            tabs.Items.Add(
            new TabItem
            {
                Header = t.Header.ToString(),
                Content = d,
                ContextMenu = c
            });
            con.Close();
            Status.Text = "Завантажено таблицю " + t.Header.ToString();
        }

        private void Table_Expanded(object sender, RoutedEventArgs e)
        {

        }




    }
}
