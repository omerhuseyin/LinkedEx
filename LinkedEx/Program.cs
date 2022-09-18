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

    /* 
       │ Author       : Omer Huseyin GUL
       │ Name         : LinkedEx
       │ GitHub       : https://github.com/omerhuseyingul
    */

    internal class Program
    {
        public enum MessageType { Error, Information, Warning}
        public static string? _accountEmailAdress;
        public static string? _accountPassword;

        public static void Main(string[] args)
        {
            Console.Title = "LinkedEx | LSA";
            bannerWriter();
            preAuthorization();
        }

        public enum GetConfigType { USERNAME, PASSWORD }

        public static void GetConfig(GetConfigType dataType)
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

        static void SaveConfig(string targetValue)
        {
            File.WriteAllText("config.json", JsonConvert.SerializeObject(new { targetValue }));
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

        public static IWebDriver driver;
        public static void SeleniumAuthorizationScript()
        {
            try
            {
                int? twoFactorCode;
                bannerWriter();

                Console.Write("[ > ] Email Address : ");
                string emailAddress = System.Console.ReadLine();

                Console.Write("[ > ] Password : ");
                string password = System.Console.ReadLine();

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

                IWebDriver driver = new ChromeDriver(service, options)
                {
                    Url = "https://www.linkedin.com/login/"
                };

                driver.FindElement(By.XPath("/html/body/div/main/div[2]/div[1]/form/div[1]/input")).SendKeys(emailAddress);
                driver.FindElement(By.XPath("/html/body/div/main/div[2]/div[1]/form/div[2]/input")).SendKeys(password);
                driver.FindElement(By.XPath("/html/body/div/main/div[2]/div[1]/form/div[3]/button")).Click();
            }
            catch (Exception)
            {
                SendMessage(mType: MessageType.Error, mContent: "Something went wrong.");
                System.Threading.Thread.Sleep(2000);
                SeleniumAuthorizationScript();
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
                SendMessage(mType: MessageType.Error, mContent: "Something went wrong.");
                System.Threading.Thread.Sleep(2000);
                preAuthorization();
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
                Environment.Exit(0);
            }
        } 
    }
}