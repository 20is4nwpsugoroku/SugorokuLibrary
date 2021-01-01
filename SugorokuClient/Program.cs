using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DxLibDLL;

namespace SugorokuClient
{
	static class Program
	{
		/// <summary>
		///  The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.SetHighDpiMode(HighDpiMode.SystemAware);
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			Form1 form = new Form1();
			form.Show();
			while (DX.ProcessMessage() != -1 && form.Created) //Application.Runしないで自分でループを作る
			{
				form.MainLoop();
				Application.DoEvents();
			}
			//Application.Run(new Form1());
		}
	}
}
