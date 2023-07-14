using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace feishu_doc_export.Helper
{
    public static class LogHelper
    {

        public static void LogInfo(string message)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"【INFO】{message}");
            Console.ResetColor();
        }

        public static void LogDebug(string message)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"【DEBUG】{message}");
            Console.ResetColor();
        }

        public static void LogWarn(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"【WARN】{message}");
            Console.ResetColor();
        }

        public static void LogError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"【ERROR】{message}（请按任意键退出！）");
            Console.ReadKey();
            Environment.Exit(0);
            Console.ResetColor();
        }

        public static void LogFatal(string message)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine($"【FATAL】{message}");
            Console.ResetColor();
        }
    }
}
