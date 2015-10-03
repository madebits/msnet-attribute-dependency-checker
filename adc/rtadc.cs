using System;
using System.Collections;
using System.Reflection;
using rtadc;

namespace adc
{
	/// <summary>
	/// Summary description for adc.
	/// </summary>
	public class adc : ICheckLogger
	{
		public adc()
		{
		}
		
		[STAThread]
		static void Main(string[] args) 
		{
			adc c = new adc();
			c.ProcessCheckCommandLine(args);
		}

		public void ProcessCheckCommandLine(string[] args)
		{
			Console.WriteLine("--- Run-Time Attribute Dependency Checker ---");
			try
			{
				int start = 0;
				bool log = false;
				if(args[start].ToLower().Equals("/l"))
				{ 
					log = true;
					start++;
				}
				string file = args[start];
				start++;
				string[] csonly = null;
				if(args.Length > start)
				{
					csonly = new string[args.Length - start];
					for(int i = 0; i < csonly.Length; i++)
						csonly[i] = args[i + start];
				}

				Assembly a = Assembly.LoadFrom(file);
				ProcessAssemblyDependencies(a, csonly, log);

			}
			catch(Exception ex)
			{
				Console.WriteLine("Error processing arguments: "
					+ ex.Message + Environment.NewLine);
				ShowHelp();
			}
		}

		private void ProcessAssemblyDependencies(
			Assembly a,
			string[] csonly,
			bool log)
		{
			try
			{
				if(log) Log(string.Empty);
				RTADCAssembly c = new RTADCAssembly();
				if(log) c.Logger = this;
				//c.Logger = this;
				
				ClassDependecyFilter f = new ClassDependecyFilter();
				f.EnableCounters = true;
				if(csonly != null)
				{
					f.ProcessTheseClassesNamesOnly = csonly;
				}
				c.Filter = f;
				try
				{
					c.Check(a);
				} 
				catch(RTADCException rtex)
				{
					Log(rtex.Message);
				}
				finally
				{
					ProcessErrors(c, f.GetCounters());
				}
			} 
			catch(Exception ex)
			{
				Log(ex.Message + Environment.NewLine);
				Log(ex.StackTrace + Environment.NewLine);
			}
		}

		private void ShowHelp()
		{
			Console.WriteLine(
				"Usage: rtadc [/L] assemblyfile [class names to process]");
			Environment.Exit(1);
		}

		private void ProcessErrors(RTAttributeDependencyChecker c, long[] counters)
		{
			Log("\r\n");
			Log("Processed: ");
			Log("|  " + counters[0] + " assembly(ies)");
			Log("|  " + counters[1] + " class(es)");
			Log("|  " + counters[2] + " method(s)");
			Log("\r\n");
			if(c.errors.HasWarnings())
			{
				Log("??? Dependency Warnings ???\r\n");
				ListArray("warning", c.errors.GetWarnings());
			}
			else Log("No warrnings!");
			if(c.errors.HasErrors())
			{
				Log("!!! Dependency Errors !!!\r\n");
				ListArray("error", c.errors.GetErrors());
			}
			else Log("No errors!");
			Log(string.Empty);
		}

		private void ListArray(string prefix, ArrayList a)
		{
			if(a == null) return;
			for(int i = 0; i < a.Count; i++)
			{
				string p = string.Empty;
				if(i < 11) p += " ";
				if(i < 101) p += " ";
				Log(p + (i + 1)
					+ " Dependency " + prefix + ": " + (string)a[i]);
			}
		}

		public void Log(string s)
		{
			Console.WriteLine(s);
		}

	} // EOC
}
