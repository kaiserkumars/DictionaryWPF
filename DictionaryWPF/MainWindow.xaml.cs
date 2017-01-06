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
using System.Windows.Navigation;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;
using System.Speech.Synthesis;

namespace DictionaryWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly SpeechSynthesizer _SS = new SpeechSynthesizer(); 
        public class word
        {
            public string dicWord { get; set; }

        }

        private MySqlConnection con;
        private MySqlCommand cmd;
        private MySqlDataReader reader;
        List<word> dbData { get; set; }
        List<word> dicData = new List<word>();
        public MainWindow()
        {
            InitializeComponent();
            AutoCompleteText();   
            Search_txt.TextChanged += new TextChangedEventHandler(Search_txt_TextChanged);
            con = new MySqlConnection();
            con.ConnectionString = @"datasource=127.0.0.1;port=3306;username=root;password=;";
        }



        //AUTO COMPLETE////AUTO COMPLETE//AUTO COMPLETE//AUTO COMPLETE//AUTO COMPLETE//AUTO COMPLETE//

        void AutoCompleteText()
        {
            //List<word> dicDataWord = new List<word>();
            String Query = "SELECT word FROM entries.dictionary";
            con = new MySqlConnection();
            con.ConnectionString = @"datasource=127.0.0.1;port=3306;username=root;password=;";
            cmd = new MySqlCommand(Query, con);
            con.Open();
            reader = cmd.ExecuteReader();
            while(reader.Read())
                {
                word newword = new word();
                newword.dicWord = reader["word"].ToString();
                
                dicData.Add(newword);
            }
            con.Close();
            this.dbData = dicData;
                     
       
           
        }

        private void Search_txt_TextChanged(object sender,TextChangedEventArgs e)
        {
            string typedString = Search_txt.Text;
            List<string> autoList = new List<string>();
            List<string> newList = new List<string>();
            newList.Clear();

            for (int i = 0; i < dbData.Count; i++)
            {
                autoList.Add(dbData[i].dicWord);
            }

            StringComparison comparision = StringComparison.InvariantCultureIgnoreCase; //Simply Awesome
            /* using the comparison parameter in StartsWith with InvarinatCultureIgnoreCase makes 
             * sure that when we compare the typedString with words from the autoList it ignores
             * the case.
             */
            foreach (string item in autoList)
            {
                if(!String.IsNullOrEmpty(Search_txt.Text))
                {
                    if(item.StartsWith(typedString,comparision))
                    {
                        newList.Add(item);
                    }
                }
            }

            if(newList.Count>0)
            {
                listBox.ItemsSource = newList;
                listBox.Visibility = Visibility.Visible;
            }
            else if (Search_txt.Text.Equals(""))
            {
                listBox.Visibility = Visibility.Collapsed;
                listBox.ItemsSource = null;
            }
            else
            {
                listBox.Visibility = Visibility.Collapsed;
                listBox.ItemsSource = null;
            }
           

            
        }

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(listBox.ItemsSource!=null)
            {
                listBox.Visibility = Visibility.Collapsed;
                Search_txt.TextChanged -= new TextChangedEventHandler(Search_txt_TextChanged);
                if(listBox.SelectedIndex!=-1)
                {
                    Search_txt.Text = listBox.SelectedItem.ToString();
                }

                Search_txt.TextChanged += new TextChangedEventHandler(Search_txt_TextChanged);
            }
        }


        //AUTO COMPLETE//AUTO COMPLETE//AUTO COMPLETE//AUTO COMPLETE//AUTO COMPLETE//AUTO COMPLETE//AUTO COMPLETE//

        //MEANING RETRIEVAL//MEANING RETRIEVAL//MEANING RETRIEVAL//MEANING RETRIEVAL//MEANING RETRIEVAL//MEANING RETRIEVAL//


        private void Search_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                con.Open();
                String Query = "SELECT * FROM entries.dictionary WHERE word='"+Search_txt.Text+"' ";
                Console.WriteLine(Query);
                cmd = new MySqlCommand(Query, con);
                reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    WordDatabase.Text = reader.GetString("word");
                    MeaningDatabase.Text = reader.GetString("definition");
                    wordTypeBlock.Text = reader.GetString("wordtype");
                }

                else
                {
                    WordDatabase.Text = "No such word found";
                    MeaningDatabase.Text = "";
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                reader.Close();
                con.Close();
            }
            

        }

        private void speech_Click(object sender, RoutedEventArgs e)
        {
            _SS.SpeakAsync(Search_txt.Text);
        }
    }
}
