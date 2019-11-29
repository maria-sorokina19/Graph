using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Model
{
    internal static class Pages
    {
        public static string GetErrorPage(string message)
        {
            return $"<html><body>Error: {message}</body></html>";
        }

        public static string GetLoginPage()
        {
            return File.ReadAllText("sites\\login.html");
        }

        public static string GetRegisterPage()
        {
            return File.ReadAllText("sites\\register.html");
        }

        public static string GetMainPage()
        {
            return File.ReadAllText("sites\\index.html");
        }

        public static string GetResultPageForLinear(double k, double b, double x1, double x2)
        {
            var values = new Dictionary<double, double>();
            for (var x = x1; x < x2; x += 0.1)
            {
                values.Add(x, k * x + b);
            }

            return GetResultPage(values);
        }

        public static string GetResultPageForQuadratic(double a, double b, double c, double x1, double x2)
        {
            var values = new Dictionary<double, double>();
            for (var x = x1; x < x2; x += 0.1)
            {
                values.Add(x, a * x * x + b * x + c);
            }

            return GetResultPage(values);
        }

        public static string GetResultPageForPower(double k, double x1, double x2)
        {
            var values = new Dictionary<double, double>();
            for (var x = x1; x < x2; x += 0.1)
            {
                values.Add(x, Math.Pow(x, k));
            }

            return GetResultPage(values);
        }

        public static string GetResultPage(Dictionary<double, double> values)
        {

            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            var width = 700;
            var height = 700;
            var stepX = values.Keys.Max() > Math.Abs(values.Keys.Min()) ? width / 2 / values.Keys.Max() : width / 2 / Math.Abs(values.Keys.Min());
            var stepY = values.Values.Max() > Math.Abs(values.Values.Min()) ? height / 2 / values.Values.Max() : height / 2 / Math.Abs(values.Values.Min());
            var step = 0.0;

            if (stepX > stepY)
            {
                step = stepY;
            }
            else
            {
                step = stepX;
            }

            var result =
                $@"<svg xmlns=""http://www.w3.org/2000/svg"" version=""1.1"" width=""{width}"" height=""{height}"">
                            <text x=""{width / 2 - 10 }"" y=""{height / 2 + 15}"">0</text>
                            <line x1 = ""{0}"" y1 = ""{height / 2}"" x2 = ""{width}"" y2 = ""{height / 2}"" stroke = ""#000"" stroke - width = ""3""   />
                            <line x1 = ""{width / 2}"" y1 = ""{0}"" x2 = ""{width / 2}"" y2 = ""{height}"" stroke = ""#000"" stroke - width = ""3"" />";
            var array = values.Keys.ToArray();
            for (var i = 1; i < array.Length; i++)
            {
                var localX = step * array[i - 1] + width / 2;
                var localY = height / 2 - step * values[array[i - 1]];
                var newlocalX = step * array[i] + width / 2;
                var newlocalY = height / 2 - step * values[array[i]];
                result += $@"<line x1 = ""{localX}"" y1 = ""{localY}"" x2 = ""{newlocalX}"" y2 = ""{newlocalY}"" stroke = ""#b4241b"" stroke - width = ""1"" />";
            }
            result += "</svg>";
            return result;
        }
    }
}

