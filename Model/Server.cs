using System;
using System.IO;
using System.Net;
using System.Text;

namespace Model
{
    public class Server
    {
        private HttpListener _httpListener;
        private HttpListenerRequest _httpListenerRequest;
        private HttpListenerContext _httpListenerContext;
        private string _hostName;
        private Database _database;

        public Database Database => _database;

        public Server()
        {
            _database = new Database();
        }

        public void InitServer()
        {
            try
            {
                _httpListener = new HttpListener();

                _hostName = "http://" + Dns.GetHostByName(Dns.GetHostName()).AddressList[0].ToString() + ":80/";
                string url = _hostName;
                _httpListener.Prefixes.Add(url);
                _httpListener.Start();
                Console.WriteLine($"Start server: {_hostName}");
                ListenPost();

            }
            catch (Exception ex)
            {
                throw new Exception("Невозможно запустить веб-сервер. Работа приложения прекращена!");
            }
        }

        private async void ListenPost()
        {
            while (true)
            {
                _httpListenerContext = await _httpListener.GetContextAsync();
                _httpListenerRequest = _httpListenerContext.Request;
                if (_httpListenerRequest.HttpMethod == "POST")
                {
                    ShowRequestData();
                }
                else
                {
                    GetRequest();
                }
            }
        }

        private void GetRequest()
        {
            Console.WriteLine(_httpListenerRequest.Url);
            switch (_httpListenerRequest.Url.LocalPath)
            {
                case "/":
                    GetMainPage();
                    break;
                case "/checkuser":
                    Console.WriteLine("CheckUserPage");
                    LogIn(_httpListenerRequest.QueryString.Get("login"), _httpListenerRequest.QueryString.Get("password"));
                    break;
                case "/reguser":
                    Console.WriteLine("RegUserPage");
                    SendPage(Pages.GetRegisterPage());
                    break;
                case "/regnewuser":
                    RegNewUser();
                    break;
                case "/getgraph":
                    switch (_httpListenerRequest.QueryString.Get("type"))
                    {
                        case "linear":
                            CalculateForLinear();
                            break;
                        case "quadratic":
                            CalculateForQuadratic();
                            break;
                        case "power":
                            CalculateForPower();
                            break;
                        default:
                            SendPage("Неизвестный тип графика");
                            break;
                    }
                    break;
                default:
                    SendPage("Ошибка 404");
                    break;
            }
        }

        private void RegNewUser()
        {
            Console.WriteLine("Reg new user thread");
            var login = _httpListenerRequest.QueryString.Get("login");
            var password = _httpListenerRequest.QueryString.Get("password");

            if (login.Length < 3)
            {
                SendPage(Pages.GetErrorPage("Длина логина должна быть больше 3 символов."));
                return;
            }

            if (password.Length < 8)
            {
                SendPage(Pages.GetErrorPage("Длина пароля должна быть больше 8 символов."));
                return;
            }

            if (_database.HasLogin(login))
            {
                SendPage(Pages.GetErrorPage("Такой логин уже существует"));
                return;
            }

            _database.RegNewUser(login, password);

            GetMainPage();
        }

        private void CalculateForLinear()
        {
            double k = 0;
            double b = 0;
            double x1 = 0;
            double x2 = 0;
            try
            {
                k = double.Parse(_httpListenerRequest.QueryString.Get("k"));
                b = double.Parse(_httpListenerRequest.QueryString.Get("b"));
                x1 = double.Parse(_httpListenerRequest.QueryString.Get("x1"));
                x2 = double.Parse(_httpListenerRequest.QueryString.Get("x2"));

                var profileId = GetIdFromCookie();
                if (profileId == -1)
                {
                    GetMainPage();
                }

                _database.AddLog(profileId, $"Draw linear graphic with coeffs = k={k} b={b} x1={x1} x2={x2}");
                SendPage(Pages.GetResultPageForLinear(k, b, x1, x2));
            }
            catch (Exception e)
            {
                SendPage(Pages.GetErrorPage("Ошибка введенных значений"));
            }
        }

        private void CalculateForQuadratic()
        {
            double a = 0;
            double b = 0;
            double c = 0;
            double x1 = 0;
            double x2 = 0;
            try
            {
                a = double.Parse(_httpListenerRequest.QueryString.Get("a"));
                b = double.Parse(_httpListenerRequest.QueryString.Get("b"));
                c = double.Parse(_httpListenerRequest.QueryString.Get("c"));
                x1 = double.Parse(_httpListenerRequest.QueryString.Get("x1"));
                x2 = double.Parse(_httpListenerRequest.QueryString.Get("x2"));

                var profileId = GetIdFromCookie();
                if (profileId == -1)
                {
                    GetMainPage();
                }

                _database.AddLog(profileId, $"Draw quadratic graphic with coeffs = a={a} b={b} c={c} x1={x1} x2={x2}");

                SendPage(Pages.GetResultPageForQuadratic(a, b, c, x1, x2));
            }
            catch (Exception e)
            {
                SendPage(Pages.GetErrorPage("Ошибка введенных значений"));
            }
        }

        private void CalculateForPower()
        {
            double k = 0;
            double x1 = 0;
            double x2 = 0;
            try
            {
                k = double.Parse(_httpListenerRequest.QueryString.Get("k"));
                x1 = double.Parse(_httpListenerRequest.QueryString.Get("x1"));
                x2 = double.Parse(_httpListenerRequest.QueryString.Get("x2"));
                var profileId = GetIdFromCookie();
                if (profileId == -1)
                {
                    GetMainPage();
                }

                _database.AddLog(profileId, $"Draw power graphic with coeffs = k={k} x1={x1} x2={x2}");

                SendPage(Pages.GetResultPageForPower(k, x1, x2));
            }
            catch (Exception e)
            {
                SendPage(Pages.GetErrorPage("Ошибка введенных значений"));
            }
        }

        
        private void LogIn(string login, string password)
        {
            if (_database.CheckUser(login, password) != -1)
            {
                SendPage(Pages.GetMainPage());
            }
            else
            {
                SendPage(Pages.GetErrorPage("Неверный логин или пароль"));
            }
        }

        private int GetIdFromCookie()
        {
            var login = GetValueFromCookie("login");
            var password = GetValueFromCookie("password");

            if (login == "" && password == "")
            {
                return -1;
            }

            return _database.CheckUser(login, password);

        }

        private void GetMainPage()
        {
            if (!CheckCookie())
            {
                SendPage(Pages.GetLoginPage());
                return;
            }

            SendPage(Pages.GetMainPage());
        }

        private bool CheckCookie()
        {
            var login = GetValueFromCookie("login");
            var password = GetValueFromCookie("password");

            if (login == "" && password == "")
            {
                return false;
            }

            if (_database.CheckUser(login, password)==-1)
            {
                return false;
            }

            return true;
        }

        public string GetValueFromCookie(string name)
        {
            string value = "";
            try
            {
                if (_httpListenerRequest.Cookies.Count > 0)
                {
                    foreach (Cookie cookie in _httpListenerRequest.Cookies)
                    {
                        if (cookie.Name == name)
                        {
                            value = cookie.Value;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                value = "";
            }

            return value;
        }

        public void SendPage(string page)
        {
            try
            {
                HttpListenerResponse response = _httpListenerContext.Response;
                response.ContentType = "text/html; charset=UTF-8";
                byte[] buffer = Encoding.UTF8.GetBytes(page);
                response.ContentLength64 = buffer.Length;
                using (Stream output = response.OutputStream)
                {
                    output.Write(buffer, 0, buffer.Length);
                }
           
            }
            catch (Exception ex)
            {
                SendPage(Pages.GetErrorPage(ex.Message));
            }
        }

        private void ShowRequestData()
        {
            if (!_httpListenerRequest.HasEntityBody)
            {
                return;
            }

            using (Stream body = _httpListenerRequest.InputStream)
            {
                using (StreamReader reader = new StreamReader(body))
                {
                }
            }
        }
    }
}
