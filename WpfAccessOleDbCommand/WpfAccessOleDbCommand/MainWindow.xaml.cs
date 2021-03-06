﻿using System;
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
using System.Data.OleDb;
using System.Data;

namespace WpfAccessOleDbCommand
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /*
         * Bağlantı nesnesi tüm form içinde kullanılabilecek şekilde tek bir konumda, sınıfın bir üyesi olarak oluşturuldu
         */ 
        OleDbConnection baglanti = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=..\\..\\Okul1.mdb");

        public MainWindow()
        {
            InitializeComponent();
        }

        public void verileriCek()
        {
            try
            {
                baglanti.Open();

                DataSet kume = new DataSet();//DataSet nesnesini oluştur
                OleDbDataAdapter da = new OleDbDataAdapter("SELECT * FROM Çalışanlar", baglanti);//DataAdapter nesnesini oluştur(sql ifadesini belirterek)
                da.Fill(kume, "Çalışanlar");//Verileri veri tabanından DataSet içindeki "Çalışanlar" tablosuna yükle

                izgara.ItemsSource = kume.Tables["Çalışanlar"].DefaultView;//DataGrid bileşeninin veri kaynağı olarak DataSet'in tablolarından "Çalışanlar" tablosunu göster
                guncelleGrid.ItemsSource = kume.Tables["Çalışanlar"].DefaultView;

                baglanti.Close();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }

        }

        private void Ekle_Click(object sender, RoutedEventArgs e)
        {
            try//Hata yakalamak için try-catch blokları kullanıldı. Bu sayede program beklemediğimiz şekilde sonlanmıyor.
            {
                baglanti.Open();//Bağlantıyı aç
                /*
                 * SQL Cümlesini oluşturuyoruz. @ ile başlayan ifadeler birer parametredir. Bu parametrelere değerler 
                 * verebiliyoruz.
                 */ 
                string sql = "INSERT INTO Çalışanlar ( TCKimlik, Adı, Soyadı, DoğumYeri, Maaş, Birimi, GirişTarihi, DoğumTarihi ) VALUES (@tckimlik, @adi, @soyadi, @dogumyeri, @maas, @birimi, @giristarihi, @dogumtarihi)";
                
                //SQL komutlarını/ifadelerini OleDbCommand kullanarak veritabanına gönderebiliyoruz.
                OleDbCommand komut = new OleDbCommand(sql, baglanti);

                //Daha önce tanımladığımız parametrelerin değerlerini ilgili bileşenden(TextBox) alarak SQL cümlesinin içine yerleşmesini sağlıyoruz
                komut.Parameters.AddWithValue("@tckimlik", tbTCKimlik.Text);
                komut.Parameters.AddWithValue("@adi", tbAdi.Text);
                komut.Parameters.AddWithValue("@soyadi", tbSoyadi.Text);
                komut.Parameters.AddWithValue("@dogumyeri", tbDogumYeri.Text);
                komut.Parameters.AddWithValue("@maas", tbMaas.Text);
                komut.Parameters.AddWithValue("@birimi", tbBirimi.Text);
                komut.Parameters.AddWithValue("@giristarihi", tbGirisTarihi.Text);
                komut.Parameters.AddWithValue("@dogumtarihi", tbDogumTarihi.Text);
                //INSERT,UPDATE ve DELETE sorguları için ExecuteNonQuery kullanıyoruz
                komut.ExecuteNonQuery();
                //İşimiz bitince bağlantıyı kapatıyoruz
                baglanti.Close();
            }
            catch (Exception exc)
            {//Hata olmuşsa hata mesajını gösterip bağlantıyı kapatıyoruz.
                MessageBox.Show(exc.Message);
                baglanti.Close();
            }
        }

        /*
         * Pencere yüklendiğinde(loaded) DataGrid bileşeninde verilerin gözükmesi için DataSet ve OleDbDataAdapter
         * bileşenleri kullanır. DataSet içinde verileri saklar, OleDbDataAdapter ise verileri veri tabanından
         * çekip DataSet'e yükler.
         */
        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            verileriCek();
        }

        private void sil_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                baglanti.Open();
                //DataGrid içindeki seçili satırı bul
                var secili = (DataRowView)izgara.SelectedItem;
                //Silme komutunu oluştur
                OleDbCommand komut = new OleDbCommand("DELETE FROM Çalışanlar WHERE TCKimlik=@tckimlik", baglanti);
                //Seçili satırın ilk sütununu(TCKimlik) SQL ifadesindeki parametre olarak ekle
                komut.Parameters.AddWithValue("@tckimlik", secili[0]);
                //Kullanıcıya silmek isteyip istemediğini sor
                var sonuc = MessageBox.Show("Silmek istiyor musunuz?", "Silme onayı", MessageBoxButton.YesNo);
                if (sonuc == MessageBoxResult.Yes)
                {
                    //silme komutunu çalıştır
                    komut.ExecuteNonQuery();
                }
                //Verileri DataGrid'e(izgara) yeniden yükle
                verileriCek();

                baglanti.Close();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        private void guncelleGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var secili = (DataRowView)guncelleGrid.SelectedItem;
                tbTCKimlikG.Text = secili[0].ToString();
                tbAdiG.Text = secili[1].ToString();
                tbSoyadiG.Text = secili[2].ToString();
                tbBirimiG.Text = secili[3].ToString();
                tbGirisTarihiG.Text = secili[4].ToString();
                tbMaasG.Text = secili[5].ToString();
                tbDogumTarihiG.Text = secili[6].ToString();
                tbDogumYeriG.Text = secili[7].ToString();

            }
            catch (Exception exc) 
            {
                
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                baglanti.Open();
                String sql = "UPDATE Çalışanlar SET Adı=@adi, Soyadı=@soyadi, Birimi=@birimi, GirişTarihi=@giristarihi, Maaş=@maas, DoğumTarihi=@dogumtarihi, DoğumYeri=@doğumyeri WHERE TCKimlik=@tckimlik";
                OleDbCommand komut = new OleDbCommand(sql, baglanti);
                
                komut.Parameters.AddWithValue("@adi", tbAdiG.Text);
                komut.Parameters.AddWithValue("@soyadi", tbSoyadiG.Text);
                komut.Parameters.AddWithValue("@birimi", tbBirimiG.Text);
                komut.Parameters.AddWithValue("@giristarihi", tbGirisTarihiG.Text);
                komut.Parameters.AddWithValue("@maas", tbMaasG.Text);
                komut.Parameters.AddWithValue("@dogumtarihi", tbDogumTarihiG.Text);
                komut.Parameters.AddWithValue("@dogumyeri", tbDogumYeriG.Text);
                komut.Parameters.AddWithValue("@tckimlik", tbTCKimlikG.Text);

                komut.ExecuteNonQuery();

                baglanti.Close();
                verileriCek();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            try
            {
                baglanti.Open();
                String sql = "SELECT * FROM Çalışanlar WHERE Adı LIKE '" + tbSorgula.Text + "%'";
                //MessageBox.Show(sql);
                //OleDbCommand komut = new OleDbCommand("SELECT * FROM Çalışanlar WHERE Adı LIKE '@adi%'", baglanti);
                //komut.Parameters.AddWithValue("@adi", tbSorgula.Text);
                DataSet kume = new DataSet();
                OleDbDataAdapter da = new OleDbDataAdapter(sql, baglanti);

                da.Fill(kume, "Çalışanlar");
                sorgulaGrid.ItemsSource = kume.Tables["Çalışanlar"].DefaultView;
                
                baglanti.Close();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }
    }
}
