using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Configuration;

namespace WindowsFormsApp1
{
	public partial class MainForm : Form
	{
		public string FilePath { get; private set; }

		// Configuration:
		private int Step = Convert.ToInt32(ConfigurationManager.AppSettings["Step"]);
		private int MinNum = Convert.ToInt32(ConfigurationManager.AppSettings["MinNum"]);
		private int MaxNum = Convert.ToInt32(ConfigurationManager.AppSettings["MaxNum"]);

		public MainForm()
		{
			InitializeComponent();
			FilePath = $"{Directory.GetCurrentDirectory()}\\CurrentPosition.txt";

			if (!File.Exists(FilePath))
			{
				CreateFile();
			}
			else
			{
				int currentNumber = GetNumberFromFile();
				SetNumberIntoFile(CheckCondition(currentNumber, MinNum, MaxNum) ? currentNumber : 0);
			}
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			RefreshMainLabel();
		}

		private void RefreshMainLabel()
		{
			label1.Text = GetNumberFromFile().ToString();
		}

		private void Form1_KeyDown(object sender, KeyEventArgs e)
		{
			int currentNumber = GetNumberFromFile();

			if (e.KeyCode == Keys.Add || e.KeyCode == Keys.Oemplus)
			{
				SetNumberIntoFile(CheckCondition(currentNumber + Step, MinNum, MaxNum) ? currentNumber + Step : currentNumber);
				RefreshMainLabel();
			}
			else if (e.KeyCode == Keys.Subtract || e.KeyCode == Keys.OemMinus)
			{
				SetNumberIntoFile(CheckCondition(currentNumber - Step, MinNum, MaxNum) ? currentNumber - Step : currentNumber);
				RefreshMainLabel();
			}
			else if (e.KeyCode == Keys.Delete)
			{
				SetNumberIntoFile(0);
				RefreshMainLabel();
			}
		}

		private void CreateFile()
		{
			if (!File.Exists(FilePath))
			{
				using (FileStream fs = File.Create(FilePath))
				{
					Byte[] info = new UTF8Encoding(true).GetBytes("0");
					fs.Write(info, 0, info.Length);
				}
			}
		}

		private int GetNumberFromFile()
		{
			using (TextReader reader = File.OpenText(FilePath))
			{
				try
				{
					return Convert.ToInt32(reader.ReadLine());
				}
				catch
				{
					reader.Close();
					File.Delete(FilePath);
					CreateFile();
					return GetNumberFromFile();
				}
			}
		}

		private void SetNumberIntoFile(int number)
		{
			using (StreamWriter sw = new StreamWriter(FilePath))
			{
				try
				{
					sw.WriteLine(number);
				}
				catch
				{
					sw.Close();
					File.Delete(FilePath);
					CreateFile();
				}
			}
		}

		Func<int, int, int, bool> CheckCondition = delegate (int number, int greaterThan, int lessThan)
		{
			return number >= greaterThan && number <= lessThan;
		};
	}
}
