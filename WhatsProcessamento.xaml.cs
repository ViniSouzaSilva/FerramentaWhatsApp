using FirebirdSql.Data.FirebirdClient;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using WpfApp1.DataSet1TableAdapters;
using WpfApp1.Properties;
using static WpfApp1.DataSet1;

namespace WpfApp1
{
    /// <summary>
    /// Lógica interna para WhatsProcessamento.xaml
    /// </summary>
    public partial class WhatsProcessamento : Window
    {
        public WhatsProcessamento()
        {
            InitializeComponent();
            //CheckLoggedIn();
            //  RunAgain();
        }

        IWebDriver driver;
        string number;
        string message;
        int PrimeiroLogin = 0;
        TRI_PDV_WHATSDataTable PRODATIVOS = new TRI_PDV_WHATSDataTable();

        private bool CheckLoggedIn()
        {
            try
            {
                return driver.FindElement(By.ClassName("_2rZZg")).Displayed;

            }
            catch (Exception e)
            {
                Debug.Print(e.Message);
                return false;
            }

        }




        public void RunAgain()
        {
            int i = 1;
            driver = new ChromeDriver();
            driver.Navigate().GoToUrl("https://web.whatsapp.com");

            while (true)
            {
                //Console.WriteLine("Login to WhatsApp Web and Press Enter");
                // Console.ReadLine();

                if (CheckLoggedIn())
                    PrimeiroLogin = +1;
                break;
            }
          //  VerificaMsg();
        }


    }
}

  //https://www.craftedforeveryone.com/whatsapp-send-api-or-click-to-chat-automation-using-selenium-and-c-sharp/
    

