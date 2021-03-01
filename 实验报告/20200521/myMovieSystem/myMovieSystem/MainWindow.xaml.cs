using System;
using System.Collections.Generic;
using System.Drawing;
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
using MySql.Data.MySqlClient;//添加注释
namespace myMovieSystem
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string Conn = "Server=localhost;User Id=root1;password=123456;Database=dbclass;Charset=utf8";
        //创建一个字符串Conn，存储数据库连接所需要的信息，包括服务器、密码、用户名、数据库名、编码方式
        private readonly int test = 1;

        void SaveImage(string hash, string name)
        {
            Bitmap newBitmap = new Bitmap(32, 32);
            string path = "C:\\Users\\MSI-PC\\Desktop\\database\\" + name;
            for( int i = 0; i < 32; ++ i )
            {
                for( int j = 0; j < 32; ++ j )
                {
                    int pos = 8 * (i / 4) + j / 4;
                
                    if( hash[pos] == '0')
                    {
                        var t = System.Drawing.Color.FromArgb(0, 0, 0);
                        newBitmap.SetPixel(i, j, t);
                    }
                    else
                    {
                        var t = System.Drawing.Color.FromArgb(255, 255, 255);
                        newBitmap.SetPixel(i, j, t);
                    }
                }
            }
            newBitmap.Save(path);
        }
        public MainWindow()
        {
            InitializeComponent();
            homePage hp = new homePage();
            if( test == 1 )
            {
                hp.Show();
                this.Close();

                var filePath = "C:\\Users\\MSI-PC\\Desktop\\database\\你的名字原版.jpg";
                var comparePath = "C:\\Users\\MSI-PC\\Desktop\\database\\你的名字旋转.jpg";
                imageSearch imgSearch = new imageSearch();
                var oP = imgSearch.getPHash(filePath);
                var oA = imgSearch.getAHash(filePath);
                var pP = imgSearch.getPHash(comparePath);
                var pA = imgSearch.getAHash(comparePath);
                var ansP = imgSearch.pHashCompare(oP, pP);
                var ansA = imgSearch.pHashCompare(oA, pA);
                double ans1 = ansP / 64.0, ans2 = ansA / 64.0;
                SaveImage(oP, "op.jpg"); SaveImage(oA, "oA.jpg");
                SaveImage(pP, "pP.jpg"); SaveImage(pA, "pA.jpg");
            }
        }

        private void login_Click(object sender, RoutedEventArgs e)
        {
            String userName = nameBox.Text;
            String userPassword = passwordBox.Password;
            if (userName == "" || userPassword == "")
            {
                MessageBox.Show("请填写用户名和密码！");
                return;
            }
            try
            {
                MySqlConnection connection = new MySqlConnection(Conn);
                connection.Open();
                string sql = "select * from db_user where name ='" + userName + "'";
                MySqlCommand cmd = new MySqlCommand(sql, connection);
                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())//如果成功读取到了数据
                {                    
                    string dbpassword = reader["password"].ToString();//获取数据库中存储的密码

                    if (userPassword != dbpassword)//如果用户输入的密码和数据库中存储的密码不相同
                    {
                        MessageBox.Show("密码错误！");
                        return;
                    }
                    else
                    {
                        homePage hp = new homePage();
                        hp.Show();
                        this.Close();
                    }
                    reader.Close();//读取完之后需要将reader关闭
                }
                else
                {
                    MessageBox.Show("该用户不存在！");
                    return;
                }
                connection.Close();//连接打开之后也要记得关闭

            }
            catch
            {
                MessageBox.Show("连接数据库失败");
                return;
            }
        }

        private void register_Click(object sender, RoutedEventArgs e)
        {
            register reg = new register();
            reg.Show();
            this.Close();
        }
    }
}
