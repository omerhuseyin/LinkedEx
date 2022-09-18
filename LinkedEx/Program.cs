namespace LinkedEx
{
    using Console = Colorful.Console;
    using System.Drawing;
    using OpenQA.Selenium;
    using System.Reflection;
    using OpenQA.Selenium.Chrome;
    using OpenQA.Selenium;
    using WebDriverManager;
    using WebDriverManager.DriverConfigs.Impl;
    using WebDriverManager.Helpers;
    using Newtonsoft.Json.Linq;
    using OpenQA.Selenium.Firefox;
    using System.Security.Cryptography.X509Certificates;
    using Newtonsoft.Json;
    using System.Diagnostics;

    /* 
       │ Author       : Omer Huseyin GUL
       │ Name         : LinkedEx
       │ GitHub       : https://github.com/omerhuseyingul
    */

    internal class Program
    {
        public enum MessageType { Error, Information, Warning}
        public enum GetConfigType { USERNAME, PASSWORD }
        public static string? _accountEmailAdress;
        public static string? _accountPassword;
        public static IWebDriver driver;

        public static void Main(string[] args)
        {
            try
            {
                DriverProcessTerminationService();
                Console.Title = "LinkedEx | LSA";
                preAuthorization();
            }
            catch (Exception)
            {
                SendMessage(mType: MessageType.Error, mContent: "Initialize error.");
                System.Threading.Thread.Sleep(2500);
                Environment.Exit(0);
            }
        }


        public static void GetConfig(GetConfigType dataType)
        {
            try
            {
                dynamic? json = JsonConvert.DeserializeObject(File.ReadAllText("config.json"));
                if (json == null)
                {
                    if (dataType == GetConfigType.USERNAME)
                        _accountEmailAdress = json.ACCOUNT_EMAILADDRESS;

                    else if (dataType == GetConfigType.PASSWORD)
                        _accountPassword = json.ACCOUNT_PASSWORD;
                }
            }
            catch (Exception)
            {
                SendMessage(mType: MessageType.Error, mContent: "[GET_ERROR] An error occurred while retrieving config information. Review the request and try again.");
                System.Threading.Thread.Sleep(2500);
                Environment.Exit(0);
            }  
        }

        static void SaveConfig(string targetValue)
        {
            try
            {
                File.WriteAllText("config.json", JsonConvert.SerializeObject(new { targetValue }));
            }
            catch (Exception)
            {
                SendMessage(mType: MessageType.Error, mContent: "[POST_ERROR] An error occurred while writing config file. Review the request and try again.");
                System.Threading.Thread.Sleep(2500);
                return;
            }
        }

        public static void bannerWriter()
        {
            Console.Clear();
            Console.Write("\n");
            string consoleBanner = @"
                

                        ██╗░░░░░██╗███╗░░██╗██╗░░██╗███████╗██████╗░███████╗██╗░░██╗
                        ██║░░░░░██║████╗░██║██║░██╔╝██╔════╝██╔══██╗██╔════╝╚██╗██╔╝
                        ██║░░░░░██║██╔██╗██║█████═╝░█████╗░░██║░░██║█████╗░░░╚███╔╝░
                        ██║░░░░░██║██║╚████║██╔═██╗░██╔══╝░░██║░░██║██╔══╝░░░██╔██╗░
                        ███████╗██║██║░╚███║██║░╚██╗███████╗██████╔╝███████╗██╔╝╚██╗
                        ╚══════╝╚═╝╚═╝░░╚══╝╚═╝░░╚═╝╚══════╝╚═════╝░╚══════╝╚═╝░░╚═╝ v1.0

                         
                        LinkedIn Skill Exam Automation | github.com/omerhuseyingul
                     All rights are free. | All responsibility belongs to the end user.
                        
            ";
            Console.WriteWithGradient(consoleBanner, Color.Purple, Color.DarkBlue, 8);
            Console.Write("\n");
        }

        public static void SeleniumAuthorizationScript()
        {
            try
            {
                int? twoFactorCode;
               
                bannerWriter();

                new DriverManager().SetUpDriver(new ChromeConfig(), VersionResolveStrategy.MatchingBrowser);

                System.Console.WriteLine("[ ! ] Transaction Pending...");
                System.Threading.Thread.Sleep(2500);
                System.Console.WriteLine("[ ! ] Please Wait...");

                ChromeDriverService service = ChromeDriverService.CreateDefaultService();
                service.EnableVerboseLogging = false;
                service.SuppressInitialDiagnosticInformation = true;
                service.HideCommandPromptWindow = true;

                ChromeOptions options = new ChromeOptions();
                //options.AddArguments(new string[] {
                //        "--disable-logging", "--mute-audio", "--disable-extensions", "--disable-notifications", "--disable-application-cache",
                //        "--no-sandbox", "--disable-crash-reporter", "--disable-dev-shm-usage", "--disable-gpu", "--ignore-certificate-errors",
                //        "--disable-infobars", "--silent" });
                //        "--disable-infobars", "--silent" });

                IWebDriver driver = new ChromeDriver(service, options)
                {
                    Url = "https://www.linkedin.com/login/"
                };

                Console.Write("[ > ] Email Address : ");
                string emailAddress = System.Console.ReadLine();

                Console.Write("[ > ] Password : ");
                string password = System.Console.ReadLine();

                driver.FindElement(By.XPath("/html/body/div/main/div[2]/div [1]/form/div[1]/input")).SendKeys(emailAddress);
                driver.FindElement(By.XPath("/html/body/div/main/div[2]/div[1]/form/div[2]/input")).SendKeys(password); TimeSpan.FromSeconds(10);
                driver.FindElement(By.XPath("/html/body/div/main/div[2]/div[1]/form/div[3]/button")).Click(); TimeSpan.FromSeconds(10);
                System.Threading.Thread.Sleep(1000);
                var isTwoFactorEnabled = driver.FindElements(By.XPath("/html/body/div/main/div[2]/h1"));

            retry2FA:

                if (isTwoFactorEnabled.Count != 0)
                {
                    Console.Write("[ > ] 2FA Code : ");
                    twoFactorCode = Int32.Parse(System.Console.ReadLine());
                    driver.FindElement(By.XPath("/html/body/div/main/div[2]/form/div[1]/input[19]")).SendKeys(twoFactorCode.ToString()); TimeSpan.FromSeconds(10);
                    driver.FindElement(By.XPath("/html/body/div/main/div[2]/form/div[2]/button")).Click(); TimeSpan.FromSeconds(10);
                    var twoFactorAuthError1 = driver.FindElements(By.XPath("/html/body/div/main/div[2]/form/div[1]/div[1]"));
                    var twoFactorAuthError2 = driver.FindElements(By.XPath("/html/body/div/main/div[1]/div[1]/span"));

                    if (twoFactorAuthError1.Count != 0 || twoFactorAuthError2.Count != 0)
                    {
                        SendMessage(mType: MessageType.Error, mContent: "Wrong Two Factor Code! Please try again!");
                        goto retry2FA;
                    }


                    else
                    {
                        System.Threading.Thread.Sleep(5000);
                        IWebElement loggedLogo = driver.FindElement(By.XPath("/html/body/div[5]/header/div/a/div/div/li-icon"));
                        if (loggedLogo.Displayed != true)
                        {
                            SendMessage(mType: MessageType.Warning, "Authorization error. Script restarting...");
                            SeleniumAuthorizationScript();
                        }

                        else
                        {
                            IWebElement accountName = driver.FindElement(By.XPath("/html/body/div[5]/div[3]/div/div/div[2]/div/div/div/div/div/a/div[2]"));
                            SendMessage(mType: MessageType.Information, mContent: $"Authorization Successfully. Logged into to {accountName.Text}");
                            Console.Title = $"LinkedEx | {accountName.Text}";
                        }
                    }
                }

            }
            catch (Exception)
            {
                SendMessage(mType: MessageType.Error, mContent: "[AUTH_ERROR] There was a problem with the authorization system. Please review the request and try again.");
                System.Threading.Thread.Sleep(2500);
                SeleniumAuthorizationScript();
            }

        }

        public static void DriverProcessTerminationService()
        {
            try
            {
                Process[] ChromeIsOpen = Process.GetProcessesByName("chromedriver");
                Process[] GeckoIsOpen = Process.GetProcessesByName("geckodriver");

                if (ChromeIsOpen.Length != 0)               
                    foreach (var process in Process.GetProcessesByName("chromedriver")) process.Kill();
                

                if (GeckoIsOpen.Length != 0)                
                    foreach (var process in Process.GetProcessesByName("geckodriver")) process.Kill();
            }
            catch (Exception) 
            {
                SendMessage(mType: MessageType.Error, mContent: "Something went wrong.");
                System.Threading.Thread.Sleep(2000);
            }

        }

        public static void preAuthorization() 
        {
            try
            {
                bannerWriter();

                string optionsMenu = @"
╔═══╦════════════════════╗
║ 1 ║ Login              ║                         
║ 2 ║ Exit               ║
╚═══╩════════════════════╝";
                System.Console.WriteLine(optionsMenu);
                System.Console.Write("[ > ] Please Make Your Choice : ");
                int choice = Int32.Parse(System.Console.ReadLine());

                switch (choice)
                {
                    default:
                        SendMessage(mType: MessageType.Error, mContent: $"{choice} is not valid.");
                        System.Threading.Thread.Sleep(2000);
                        preAuthorization();
                        break;

                    case 1:
                        SeleniumAuthorizationScript();
                        break;

                    case 2:
                        bannerWriter();
                        SendMessage(mType: MessageType.Information, mContent: "Shutting down...");
                        System.Threading.Thread.Sleep(1000);
                        Environment.Exit(0);
                        break;
                }
            }
            catch (Exception)
            {
                SendMessage(mType: MessageType.Error, mContent: "[GENERAL_ERROR] An unknown error has occurred. Please try the request again.");
                System.Threading.Thread.Sleep(2500);
                SeleniumAuthorizationScript();
            }
            
        }

        public static void SendMessage(MessageType mType, string mContent)
        {
            try
            {
                switch (mType)
                {
                    default: 
                        Environment.Exit(0);
                        break;

                    case MessageType.Error:
                        Console.ForegroundColor = Color.DarkRed;
                        System.Console.WriteLine("[ X ] {0}", mContent);
                        Console.ResetColor();
                        break;

                    case MessageType.Information:
                        Console.ForegroundColor = Color.Aqua;
                        System.Console.WriteLine("[ i ] {0}", mContent);
                        Console.ResetColor();
                        break;

                    case MessageType.Warning:
                        Console.ForegroundColor = Color.Yellow;
                        System.Console.WriteLine("[ ! ] {0}", mContent);
                        Console.ResetColor();
                        break;
                }
            }

            catch (Exception)
            {
                SendMessage(mType: MessageType.Error, mContent: "[TELEMETRY_ERROR] An error occurred for the output. Please contact the developer.");
                System.Threading.Thread.Sleep(2500);
                SeleniumAuthorizationScript();
            }
        } 
    }
}