using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Threading;

namespace auto_tests
{
	class Program
	{
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern uint FindWindow(string lpClassName, string lpWindowName);
		
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern uint FindWindowEx(uint hwndParent, uint hwndChildAfter, string className, string windowName);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern int SendMessage(uint hWnd, int msg, int wParam, int lParam);


		//Определение констант
		const int WM_LBUTTONDOWN = 0x0201;
		const int WM_LBUTTONUP = 0x0202;
		const int WM_KEYDOWN = 0x0100;
		const int WM_KEYUP = 0x0101;
		const int WM_CHAR = 0x0102;
		const int BTN_CLICK = 0x00F5;
		const int WM_GETTEXT = 0x000D;
		const int WM_GETTEXTLENGTH = 0x000E;
		const int WM_SETTEXT = 0x000C;


		static void Main(string[] args)
		{
			uint calc_window = FindWindowEx(0, 0, "Tjform", "CALC");

			// numbers
			Dictionary<int, uint> num_pad_dic = new Dictionary<int, uint>();
			uint num_1 = FindWindowEx(calc_window, 0, "TButton", "1");
			uint num_2 = FindWindowEx(calc_window, 0, "TButton", "2");
			uint num_3 = FindWindowEx(calc_window, 0, "TButton", "3");
			uint num_4 = FindWindowEx(calc_window, 0, "TButton", "4");
			uint num_5 = FindWindowEx(calc_window, 0, "TButton", "5");
			uint num_6 = FindWindowEx(calc_window, 0, "TButton", "6");
			uint num_7 = FindWindowEx(calc_window, 0, "TButton", "7");
			uint num_8 = FindWindowEx(calc_window, 0, "TButton", "8");
			uint num_9 = FindWindowEx(calc_window, 0, "TButton", "9");
			uint num_0 = FindWindowEx(calc_window, 0, "TButton", "0");
			var num_pad_array = new uint[] { num_0, num_1, num_2, num_3, num_4, num_5, num_6, num_7, num_8, num_9 };			
			for (int i = 0; i < num_pad_array.Length; i++)
			{
				num_pad_dic.Add(i, num_pad_array[i]);
			}

			// special symbols
			uint dot = FindWindowEx(calc_window, 0, "TButton", ".");

			// functions
			Dictionary<string, uint> functions_dic = new Dictionary<string, uint>();			
			uint plus = FindWindowEx(calc_window, 0, "TButton", "+");
			functions_dic.Add("+", plus);
			uint minus = FindWindowEx(calc_window, 0, "TButton", "-");
			functions_dic.Add("-", minus);
			uint multiply = FindWindowEx(calc_window, 0, "TButton", "*");
			functions_dic.Add("*", multiply);
			uint divide = FindWindowEx(calc_window, 0, "TButton", "/");
			functions_dic.Add("/", divide);


			// functional buttons
			uint equale = FindWindowEx(calc_window, 0, "TButton", "=");
			uint clear = FindWindowEx(calc_window, 0, "TButton", "C");
			
			// Result
			uint res_display = FindWindowEx(calc_window, 0, "TEdit", null);

			const int numberPrecision = 3;

			for (int i = 0; i < 4; i++) {

				foreach (var operation in functions_dic)
				{
					foreach (var btn1 in num_pad_dic)
					{
						foreach (var btn2 in num_pad_dic)
						{
							if (operation.Key == "/" && btn2.Key == 0) { continue; }
							SendMessage(clear, BTN_CLICK, 0, 0);
							SendMessage(btn1.Value, BTN_CLICK, 0, 0);
							if (i == 2 || i == 3)
							{
								SendMessage(dot, BTN_CLICK, 0, 0);
								SendMessage(btn2.Value, BTN_CLICK, 0, 0);
							}
							SendMessage(operation.Value, BTN_CLICK, 0, 0);
							SendMessage(btn2.Value, BTN_CLICK, 0, 0);
							if (i == 1 || i == 3) 
							{
								SendMessage(dot, BTN_CLICK, 0, 0);
								SendMessage(btn1.Value, BTN_CLICK, 0, 0);
							}
							SendMessage(equale, BTN_CLICK, 0, 0);

							var calculatorResult = GetControlText((IntPtr)res_display);

							double trueResult = 0;
							double firstNumber = i == 2 
								? Double.Parse(btn1.Key + "." + btn2.Key) 
								: i == 3 
									? Double.Parse(btn1.Key + "." + btn2.Key) 
									: btn1.Key;
							double secondNumber = i == 1 
								? Double.Parse(btn2.Key + "." + btn1.Key) 
								: i == 3 
									? Double.Parse(btn2.Key + "." + btn1.Key) 
									: btn2.Key;

							if (operation.Key == "+")
							{
								trueResult = Math.Round((double)(firstNumber + secondNumber), numberPrecision);
							}
							else if (operation.Key == "-")
							{
								trueResult = Math.Round((double)(firstNumber - secondNumber), numberPrecision);
							}
							else if (operation.Key == "/")
							{
								trueResult = Math.Round((double)(firstNumber / secondNumber), numberPrecision);
							}
							else if (operation.Key == "*")
							{
								trueResult = Math.Round((double)(firstNumber * secondNumber), numberPrecision);
							}

							double finalCalcRes = Math.Round(Double.Parse(calculatorResult), numberPrecision);
							bool isEqual = finalCalcRes == trueResult;

                            //if (!isEqual) {
                            //	Console.WriteLine(
                            //		isEqual
                            //		+ "\t" +
                            //		btn1.Key + " " + operation.Key + " " + secondNumber + " = " + calculatorResult.ToString()
                            //		+ "\t" +
                            //		btn1.Key + " " + operation.Key + " " + secondNumber + " = " + trueResult.ToString()
                            //		+ "\n-------------------------------------------------------------------------------"
                            //	);
                            //}
                            Console.WriteLine(
                                isEqual
                                + "\t" +
								firstNumber + " " + operation.Key + " " + secondNumber + " = " + finalCalcRes.ToString()
                                + "\t" +
								firstNumber + " " + operation.Key + " " + secondNumber + " = " + trueResult.ToString()
                                + "\n-------------------------------------------------------------------------------"
                            );
                        }
					}
				}
			}

			Console.ReadLine();
		}


		[System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "SendMessage", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
		public static extern bool SendMessage(IntPtr hWnd, uint Msg, int wParam, StringBuilder lParam);

		[System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr SendMessage(int hWnd, int Msg, int wparam, int lparam);

		public static string GetControlText(IntPtr hWnd)
		{

			// Get the size of the string required to hold the window title (including trailing null.) 
			Int32 titleSize = SendMessage((int)hWnd, WM_GETTEXTLENGTH, 0, 0).ToInt32();

			// If titleSize is 0, there is no title so return an empty string (or null)
			if (titleSize == 0)
				return String.Empty;

			StringBuilder title = new StringBuilder(titleSize + 1);

			SendMessage(hWnd, (int)WM_GETTEXT, title.Capacity, title);

			return title.ToString();
		}
	}
}
