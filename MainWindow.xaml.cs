using FirebirdSql.Data.FirebirdClient;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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
using WpfApp1.DataSet1TableAdapters;
using WpfApp1.Properties;
using static WpfApp1.DataSet1;
using static WpfApp1.Funcoes;

namespace WpfApp1
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class MainWindow : Window
    {
        IWebDriver driver;
        string number;
        string message;
        int PrimeiroLogin = 0;
        bool TelaNumIncorreto = false ;
        TRI_PDV_WHATSDataTable PRODATIVOS = new TRI_PDV_WHATSDataTable();

        public MainWindow()
        {
            InitializeComponent();
            CheckLoggedIn();
            RunAgain();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            while (true)
            {
               // Console.WriteLine("Login to WhatsApp Web and Press Enter");
              //  Console.ReadLine();

                if (CheckLoggedIn())
                    PrimeiroLogin = +1;
                iniciar_btn.Visibility = Visibility.Visible;
                break;
            }

            VerificaMsg();

        }
     

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

            
           
        }

        private bool SendMessage(string number, string message, int id)
        {
            try
            {
               
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
                TimeSpan intervalo = new TimeSpan(0, 0, 5);
               // wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.Id("action-button")));
                
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10); //Wait for maximun of 10 seconds if any element is not found
                driver.Navigate().GoToUrl("https://api.whatsapp.com/send?phone=" + number + "&text=" + Uri.EscapeDataString(message));
                try
                {
                    Thread.Sleep(intervalo);
                    wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.Id("action-button")));
                    driver.FindElement(By.Id("action-button")).Click(); // Click SEND Buton
                }
                catch (Exception ex)
                {
                    if (ex.Message.Equals("Timed out after 20 seconds"))
                    {
                        Thread.Sleep(intervalo);
                        driver.FindElement(By.XPath("//*[@id='action - button']")).Click();

                        using (var msg = new TRI_PDV_WHATSTableAdapter())
                        {
                            msg.Connection.ConnectionString = MontaStringDeConexao(Properties.Settings.Default.ServerName, Properties.Settings.Default.ServerCatalog);

                            msg.GravaMsgComErro(id);
                        }
                        return false;
                    }

                } 
                
                
                try
                {
                    Thread.Sleep(intervalo);
                    wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.XPath("//*[@id='fallback_block']/div/div/a")));
                    driver.FindElement(By.XPath("//*[@id='fallback_block']/div/div/a")).Click();
                }
                catch (Exception ex) 
                {
                    if (ex.Message.Equals("Timed out after 20 seconds"))
                    {
                        Thread.Sleep(intervalo);
                        driver.FindElement(By.XPath("//*[@id='action - button']")).Click();

                        using (var msg = new TRI_PDV_WHATSTableAdapter())
                        {
                            msg.Connection.ConnectionString = MontaStringDeConexao(Properties.Settings.Default.ServerName, Properties.Settings.Default.ServerCatalog);

                            msg.GravaMsgComErro(id);
                        }
                        return false;
                    }

                }
                //wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.XPath("//*[@id='fallback_block']/div/div/a")));
                //driver.FindElement(By.XPath("//*[@id='fallback_block']/div/div/a")).Click();
                //driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(1);

                try 
                {

                    Thread.Sleep(intervalo);
                    wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.XPath("//*[@id='main']/footer/div[1]/div[3]")));
                    driver.FindElement(By.XPath("//*[@id='main']/footer/div[1]/div[3]")).Click();//Click SEND Arrow Button

                    using (var msg = new TRI_PDV_WHATSTableAdapter())
                    {
                        msg.GravaMsgComSucesso(id);
                    }
                    Thread.Sleep(intervalo);
                    wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.XPath("//*[@id='main']/div[3]/div/div/div[3]/div[18]/div/div/div/div[2]/div/div/span")));
                }
                catch (Exception ex)
                {
                    
                    if (ex.Message.Contains("no such element: Unable to locate element: ") || ex.Message.Contains("Timed out after 20 seconds"))//{\"method\":\"xpath\",\"selector\":\"//*[@id='app']/div/span[2]/div/span/div/div/div/div/div/div[1]\"}\n"))
                    {
                        Thread.Sleep(intervalo);
                        wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.XPath("//*[@id='app']/div/span[2]/div/span/div/div/div/div")));
                        IWebElement element = driver.FindElement(By.XPath("//*[@id='app']/div/span[2]/div/span/div/div/div/div/div/div[1]"));

                        if (element.Displayed == true)
                        {


                            try
                            {

                                driver.FindElement(By.XPath("//*[@id='app']/div/span[2]/div/span/div/div/div/div/div/div[2]/div")).Click();
                                using (var msg = new TRI_PDV_WHATSTableAdapter())
                                {
                                    msg.Connection.ConnectionString = MontaStringDeConexao(Properties.Settings.Default.ServerName, Properties.Settings.Default.ServerCatalog);

                                    msg.GravaMsgComErro(id);
                                }
                                return false;
                            }
                            catch (Exception exc)
                            {
                                MessageBox.Show("Não foi possível continuar! \n Reinicie o programa e tente novamente", "Atenção");
                                using (var msg = new TRI_PDV_WHATSTableAdapter())
                                {
                                    msg.Connection.ConnectionString = MontaStringDeConexao(Properties.Settings.Default.ServerName, Properties.Settings.Default.ServerCatalog);

                                    msg.GravaMsgComErro(id);


                                }
                                return false;

                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                
                //MessageBox.Show("Não foi possível encaminhar o arquivo\n Verifique se o número de telefone está correto", "Atenção");
                using (var msg = new TRI_PDV_WHATSTableAdapter())
                {
                    msg.Connection.ConnectionString = MontaStringDeConexao(Properties.Settings.Default.ServerName, Properties.Settings.Default.ServerCatalog);

                    msg.GravaMsgComErro(id);
                }
                VerificaMsg();
            }
            return true;
        }

        public void VerificaMsg()
        {
            var novo_envio = new FbRemoteEvent(MontaStringDeConexao(Properties.Settings.Default.ServerName, Properties.Settings.Default.ServerCatalog));
            //while ( /*Process.GetProcessesByName("AmbiPDV").Length >= 1*/)

            novo_envio.RemoteEventCounts += (sender, e) =>
            {

                using (var ATIVOS = new TRI_PDV_WHATSTableAdapter())
                {
                    ATIVOS.Connection.ConnectionString = MontaStringDeConexao(Properties.Settings.Default.ServerName, Properties.Settings.Default.ServerCatalog);

                    ATIVOS.GetRegistroDeMsg(PRODATIVOS);
                    foreach (TRI_PDV_WHATSRow item in PRODATIVOS)
                    {
                        var num = item.NUMERO;
                        var cfe = item.MENSAGEM;
                        var id = item.ID_MSG;
                        Console.WriteLine("Digite o Numero com DDD");
                        // number = "5511986093045"; //Console.ReadLine();
                        Console.WriteLine("Digite a mensagem");
                        //message = "Oi";//Console.ReadLine();
                        SendMessage(num, cfe, id); //*** Replace here ***//
                                                   
                    }
                }
            };

            novo_envio.QueueEvents("NOVA_MENSAGEM");

        }
    }

}
//https://www.craftedforeveryone.com/whatsapp-send-api-or-click-to-chat-automation-using-selenium-and-c-sharp/


