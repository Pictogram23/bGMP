using System;
using System.Windows.Forms;
using System.IO;
using static bGMP.GlobalParams;

namespace bGMP
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length > 1 && args[0] != PASSWORD)
            {
                Environment.Exit(0);
            }

            using (
                FileStream fileStream = File.Create(BGMP_INI))
            {
                fileStream.Close();
            }

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            //ApplicationConfiguration.Initialize();
            new Form1();
            Application.Run();
        }
    }
}