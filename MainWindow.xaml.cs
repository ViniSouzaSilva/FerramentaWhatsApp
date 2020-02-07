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
             

                if (CheckLoggedIn())
                    PrimeiroLogin = +1;
                iniciar_btn.Visibility = Visibility.Visible;
                break;
            }

            VerificaMsg();

        }
     
        /// <summary>
        /// Checa se o whatsApp foi logado 
        /// </summary>
        /// <returns></returns>
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
        /// <summary>
        /// Inicia uma pagina web no site o WhatsApp Web 
        /// </summary>
        public void RunAgain()
        {
            int i = 1;
            driver = new ChromeDriver();
            driver.Navigate().GoToUrl("https://web.whatsapp.com");

            
           
        }
        /// <summary>
        /// Utiliza da ferramenta Selenium para acessar o WhatsApp e enviar as mensagens 
        /// </summary>
        /// <param name="number"></param>
        /// <param name="message"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool SendMessage(string number, string message, int id)
        {
            /* Todo o fluxo tem de ser seguido de maneira correta para não interferir no envio das mensagens, por esse motivo foi necessário colocar
            tantas verificações e try catch*/
            try
            {
                //Instância a Função wait que será utilizada posteriormente para aguardar as telas certas aparecerem
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));

                //Define uma função que espera um intervalo de tempo para a continuidade do código (Diferente da função Wait)
                TimeSpan intervalo = new TimeSpan(0, 0, 5);

                //Espera qualquer coisa aparecer no navegador dentro de 10 segundos
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10); //Wait for maximun of 10 seconds if any element is not found

                //Coloca a URL da API dos whats
                driver.Navigate().GoToUrl("https://api.whatsapp.com/send?phone=" + number + "&text=" + Uri.EscapeDataString(message));
                try
                {

                    Thread.Sleep(intervalo);// timer para o programa ir de forma mais lenta  

                    //Essa função aguarda o componente do site está aparecendo para continuar, caso não esteja, será gerado uma exceção se será tratada
                    wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.Id("action-button")));

                    //Clica na opção apontada 
                    driver.FindElement(By.Id("action-button")).Click(); // Click SEND Buton
                }
                catch (Exception ex)
                {
                    // em caso se exceção
                    if (ex.Message.Equals("Timed out after 20 seconds"))
                    {
                        Thread.Sleep(intervalo);
                        driver.FindElement(By.XPath("//*[@id='action - button']")).Click();

                        using (var msg = new TRI_PDV_WHATSTableAdapter())
                        {
                            msg.Connection.ConnectionString = MontaStringDeConexao(Properties.Settings.Default.ServerName, Properties.Settings.Default.ServerCatalog);

                            // Grava o mensagem como não enviada por conta de algum erro 
                            msg.GravaMsgComErro(id);
                        }
                        return false;
                    }

                } 
                
                /* essas verficações são repetidas de maneira adaptada para cada tela */
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
                

                try 
                {

                    Thread.Sleep(intervalo);
                    wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.XPath("//*[@id='main']/footer/div[1]/div[3]")));
                    driver.FindElement(By.XPath("//*[@id='main']/footer/div[1]/div[3]")).Click();//Click SEND Arrow Button

                    using (var msg = new TRI_PDV_WHATSTableAdapter())
                    {
                        // Salva no registroque foi enviada corretamente
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
                
                using (var msg = new TRI_PDV_WHATSTableAdapter())
                {
                    msg.Connection.ConnectionString = MontaStringDeConexao(Properties.Settings.Default.ServerName, Properties.Settings.Default.ServerCatalog);

                    msg.GravaMsgComErro(id);
                }
                VerificaMsg();
            }
            return true;
        }
        /// <summary>
        /// Verifica os envios das mesagens e aguarda o evento de inserção de uma nova mensagem
        /// </summary>
        public void VerificaMsg()
        {
            //Instância um visualizador de evento remoto e pega a string de conexão da base de dados
            var novo_envio = new FbRemoteEvent(MontaStringDeConexao(Properties.Settings.Default.ServerName, Properties.Settings.Default.ServerCatalog));
            
            // Dentro desse bloco de comando, existem instruções que serão executadas quando o evento for realizado
            novo_envio.RemoteEventCounts += (sender, e) =>
            {

                using (var ATIVOS = new TRI_PDV_WHATSTableAdapter())
                {
                    // Força o objeto a enxergar a string de conexão salva nas propriedades 
                    ATIVOS.Connection.ConnectionString = MontaStringDeConexao(Properties.Settings.Default.ServerName, Properties.Settings.Default.ServerCatalog);

                    //Pega apenas as mensagens que não foram enviadas
                    ATIVOS.GetRegistroDeMsg(PRODATIVOS);
                   
                    //Para cada mensagem não enviada, faça isso
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
            //Fica em StandBy aguardando o evento com o nome (NOVA_MENSAGEM) 
            novo_envio.QueueEvents("NOVA_MENSAGEM");

        }
    }

}
//https://www.craftedforeveryone.com/whatsapp-send-api-or-click-to-chat-automation-using-selenium-and-c-sharp/


