using System;
using System.Windows.Forms;
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


            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            //ApplicationConfiguration.Initialize();
            new Form1();
            Application.Run();
        }
    }
}