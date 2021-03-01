using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using MySql.Data.MySqlClient;
namespace myMovieSystem
{

    class Martrix
    {
        public int width, height;
        public double[,] martrix;
        public Martrix( int h = 32, int w = 32 )
        {
            width = w; height = h;
            martrix = new double[h, w];
            for (int i = 0; i < height; ++i)
                for (int j = 0; j < width; ++j)
                    martrix[i, j] = 0;
        }
        public Martrix( int[,] array )
        {
            height = array.GetLength(0); width = array.GetLength(1);
            martrix = new double[height, width];
            for ( int i = 0; i < height; ++ i )
                for( int j = 0; j < width; ++ j )
                    martrix[i, j] = (double)(array[i, j]);
        }

        public Martrix multy(Martrix b)
        {
            Martrix ans = new Martrix( height, b.width );
            for( int i = 0; i < height; ++ i )
                for( int j = 0; j < b.width; ++ j )
                    for( int k = 0; k < width; ++ k )
                        ans.martrix[i, j] += martrix[i, k] * b.martrix[k,j];
            return ans;
        }
        public void Reverse()
        {
            double[,] tmp = new double[width, height];
            for( int i = 0; i < height; ++ i )
                for( int j = 0; j < width; ++ j )
                    tmp[j, i] = martrix[i, j];
            width ^= height;
            height ^= width;
            width ^= height;
            martrix = tmp;
        }
        public void GetDctMartrix( int N )
        {
            width = height = N;
            double a;
            martrix = new double[N, N];
            for( int i = 0; i < N; ++ i )
            {
                for( int j = 0;  j < N; ++ j )
                {
                    if (i == 0) a = Math.Sqrt(1.0 / N);
                    else a = Math.Sqrt(2.0 / N);
                    martrix[i, j] = a * Math.Cos((j + 0.5) * Math.PI * i / N);
                }
            }
        }
    }
    class imageSearch
    {
        public static string Conn = "Server=localhost;User Id=root1;password=123456;Database=dbclass;Charset=utf8";
        //获取图像灰度值
        

        private int filter( double x )
        {
            if (x < 0) return 0;
            if (x > 255) return 255;
            return (int)x;
        }
        public int[,] getGrayArray( Bitmap curBitmap )
        {
            int height = curBitmap.Height, width = curBitmap.Width;
            int[,] gray = new int[height, width];
            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    var curColor = curBitmap.GetPixel(i, j);//获取当前像素的颜色值
                    //转化为灰度值
                    int m = (int)(curColor.R * 0.299 + curColor.G * 0.587 + curColor.B * 0.114);
                    if (m > 255) m = 255;
                    if (m < 0) m = 0;
                    gray[j, i] = m;
                }
            }
            return gray;
        }
        public Bitmap shrink( string filename, int nW = 32, int nH = 32, string name = "saved_img.jpg" )
        {
            Bitmap curBitmap = (Bitmap)Image.FromFile(filename);
            Bitmap newBitmap = new Bitmap(nW, nH);
            int width = curBitmap.Width, height = curBitmap.Height;
            double fw = nW / (double)(width), fh = nH / (double)(height);

            for (int i = 0; i < nH; ++i)
            {
                for (int j = 0; j < nW; ++j)
                {
                    double posW = j / fw, posH = i / fh;
                    int a_w = (int)(posW), a_h = (int)(posH + 1);
                    int d_w = (int)(posW), d_h = (int)(posH);
                    int b_w = (int)(posW + 1), b_h = (int)(posH + 1);
                    if (b_h >= height){ a_h--; b_h--; }
                    int c_w = (int)(posW + 1), c_h = (int)(posH);
                    if (c_w >= width) { c_w--; b_w--; }
                    Color a = curBitmap.GetPixel(a_w, a_h);
                    Color b = curBitmap.GetPixel(b_w, b_h);
                    Color c = curBitmap.GetPixel(c_w, c_h);
                    Color d = curBitmap.GetPixel(d_w, d_h);
                    double ans_up_r, ans_down_r, ans_r;
                    double ans_up_b, ans_down_b, ans_b;
                    double ans_up_g, ans_down_g, ans_g;

                    ans_up_r = 1.0 * (a.R - b.R) * (posW - a_w) * (posW - a_w) + a.R;
                    ans_down_r = 1.0 * (c.R - d.R) * (posW - d_w) * (posW - d_w) + d.R;
                    ans_r = (ans_up_r - ans_down_r) * (posH - c_h) + ans_down_r;

                    ans_up_g = 1.0 * (a.G - b.G) * (posW - a_w) * (posW - a_w) + a.G;
                    ans_down_g = 1.0 * (c.G - d.G) * (posW - d_w) * (posW - d_w) + d.G;
                    ans_g = (ans_up_g - ans_down_g) * (posH - c_h) + ans_down_g;

                    ans_up_b = 1.0 * (a.B - b.B) * (posW - a_w) * (posW - a_w) + a.B;
                    ans_down_b = 1.0 * (c.B - d.B) * (posW - d_w) * (posW - d_w) + d.B;
                    ans_b = (ans_up_b - ans_down_b) * (posH - c_h) + ans_down_b;

                    newBitmap.SetPixel(j, i, Color.FromArgb(filter(ans_r), filter(ans_g), filter(ans_b)) );
                    //ans_up = 1.0 * (inBuffer[at(b_h, b_w)] - inBuffer[at(a_h, a_w)]) * (posW - a_w) + inBuffer[at(a_h, a_w)];
                    //ans_down = 1.0 * (inBuffer[at(c_h, c_w)] - inBuffer[at(d_h, d_w)]) * (posW - d_w) + inBuffer[at(d_h, d_w)];
                    //ans = (ans_up - ans_down) * (posH - d_h) + ans_down;
                }
            }
            string path = "C:\\Users\\MSI-PC\\Desktop\\database\\" + name;
            var sBitmap = new Bitmap(curBitmap, 50, 50);
            sBitmap.Save(path);
            return newBitmap;
        }
        public Martrix dctTrans( int[,] gray )
        {
            Martrix dctMartrix = new Martrix(), grayMartrix = new Martrix(gray);
            dctMartrix.GetDctMartrix(32);
            var ans = dctMartrix.multy(grayMartrix);
            dctMartrix.Reverse();
            ans = ans.multy(dctMartrix);
            return ans;
        }
        public double[,] getDcCoff( Martrix dctCoff )
        {
            double[,] ans = new double[8, 8];
            for( int i = 0; i < 8; ++ i )
                for( int j = 0; j < 8; ++ j )
                    ans[i, j] = dctCoff.martrix[i, j];
            return ans;
        }
        public double getAve( double[,] dcCoff )
        {
            int width = dcCoff.GetLength(1), height = dcCoff.GetLength(0);
            double ave = 0;
            for (int i = 0; i < height; ++i)
                for (int j = 0; j < width; ++j)
                    ave += dcCoff[i, j];
            ave /= width * height;
            return ave;
        }
        public string getPHash(string filename)
        {
            string ans = "";
            var smallBitMap = shrink(filename);
            var gray = getGrayArray(smallBitMap);
            var dctCoff = dctTrans(gray);
            var dcCoff = getDcCoff(dctCoff);
            var ave = getAve(dcCoff);
            for( int i = 0; i < 8; ++ i )
            {
                for( int j = 0; j < 8; ++ j )
                {
                    if (dcCoff[i, j] < ave)
                        ans += "1";
                    else ans += "0";
                }
            }
            return ans;
        }
        public string getAHash(string filename)
        {
            string ans = "";
            var smallBitMap = shrink(filename, 8, 8);
            var gray = getGrayArray(smallBitMap);
            double ave = 0;
            for (int i = 0; i < 8; ++i)
                for (int j = 0; j < 8; ++j)
                    ave += gray[i, j];
            ave /= 64;
            for (int i = 0; i < 8; ++i)
            {
                for (int j = 0; j < 8; ++j)
                {
                    if (gray[i, j] < ave)
                        ans += "1";
                    else ans += "0";
                }
            }
            return ans;
        }
        public int pHashCompare( string a, string b )
        {
            int cnt = 0;
            for (int i = 0; i < a.Length; ++i)
                if (a[i] == b[i]) cnt++;
            return cnt;
        }
        public string getGray(string filename)
        {
            Bitmap curBitmap = (Bitmap)Image.FromFile(filename);
            int width = curBitmap.Width;
            int height = curBitmap.Height;
            Color curColor;
            int[] gray = new int[256];
            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    curColor = curBitmap.GetPixel(i,j);//获取当前像素的颜色值
                    //转化为灰度值
                    int m = (int)(curColor.R * 0.299 + curColor.G * 0.587 + curColor.B * 0.114);
                    if (m > 255) m = 255;
                    if (m < 0) m = 0;
                    gray[m]++;//该数组存放的是某一灰度出现次数。上面出现了m，所以灰度m的出现次数要加一。
                }
            }
            string g = string.Join(",", gray);//将数组转化为字符串存储
            return g;
        }
        //比较灰度直方图
        public int compare(string sqlImageGray, string upImageGray)
        {
            int count = 0;
            string [] sqlGray = sqlImageGray.Split(',');
            string[] upGray = upImageGray.Split(',');
            for (int i = 0; i < 256; i++)
                if (sqlGray[i] == upGray[i])
                    count++;
            /* 比较的方法：
             * 遍历每一个灰度等级，比较二者的出现频率是否一致，如果不一致则count计数加1
             * 返回值意义：
             * 返回出现频率不一样的灰度等级的个数
             */
            return count;
        }
        //搜索数据库，按相似度顺序返回比较结果
        public int[] pHashSearchResult(string pHash)
        {
            int[] imgOrder = new int[100];
            int n = 0;
            string sql = "select img_info from db_movie_info";
            MySqlConnection conn = new MySqlConnection(Conn);
            conn.Open();
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                imgOrder[n++] = pHashCompare(reader["img_info"].ToString(), pHash);
                //读取每一张图片的灰度信息，与上传的信息相比较，得到有多少灰度等级的频率不同
                //最后将比较结果按照顺序存放在imgOrder里边
            }
            reader.Close();
            int[] index = sortDistance(imgOrder);//调用自定义排序方法
            return index;
        }
        public int[] searchResult(string upGray)
        {
            int[] imgOrder = new int[100];
            int n = 0;
            string sql = "select img_info from db_movie_info";
            MySqlConnection conn = new MySqlConnection(Conn);
            conn.Open();
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                imgOrder[n++] = compare(reader["img_info"].ToString(), upGray);
                //读取每一张图片的灰度信息，与上传的信息相比较，得到有多少灰度等级的频率不同
                //最后将比较结果按照顺序存放在imgOrder里边
            }
            reader.Close();
            int[] index = sortGray(imgOrder);//调用自定义排序方法
            return index;
        }

        private int[] sortDistance(int[] imgOrder)
        {
            int len = imgOrder.Length;
            int[] index = new int[len];
            int temp;
            for (int i = 0; i < len; i++)
                index[i] = i;//用于存储下标
            for (int j = 0; j < len; j++)
                for (int i = 0; i < len - j - 1; i++)//降序排序
                {
                    if (imgOrder[i] < imgOrder[i + 1])
                    {
                        //交换数值
                        temp = imgOrder[i];
                        imgOrder[i] = imgOrder[i + 1];
                        imgOrder[i + 1] = temp;
                        //交换下标
                        temp = index[i];
                        index[i] = index[i + 1];
                        index[i + 1] = temp;
                    }
                }
            return index;
        }
        //排序，索引不变排序法
        private int[] sortGray(int[] imgOrder)
        {
            int len = imgOrder.Length;
            int[] index = new int[len];
            int temp ;
            for (int i = 0; i < len; i++)
                index[i] = i;//用于存储下标
            for (int j = 0; j < len; j++)
                for (int i = 0; i < len - j - 1; i++)//降序排序
                {
                    if (imgOrder[i] < imgOrder[i + 1])
                    {
                        //交换数值
                        temp = imgOrder[i];
                        imgOrder[i] = imgOrder[i + 1];
                        imgOrder[i + 1] = temp;
                        //交换下标
                        temp = index[i];
                        index[i] = index[i + 1];
                        index[i + 1] = temp;
                    }
                }
            return index;
        }

    }
}
