using System;
using System.Collections;

namespace rtadc
{
	public class ErrorReport
	{
		private ArrayList errors;
		private ArrayList warnings;
		private bool stopOnError = false;
		// where we are in the program tree:
		private ArrayList nameCtx = new ArrayList();
		private bool stopOnDAUsageError = true;
		
		public ErrorReport()
		{
		}

		public void EnterContext(string s)
		{
			nameCtx.Add(s);
		}

		public void LeaveContext()
		{
			DependencyUtils.RemoveLastElement(ref nameCtx);
		}

		public void AddError(string err)
		{
			if(errors == null) errors = new ArrayList();
			errors.Add(err + " @ " + GetLocation());
			if(StopOnError)
				throw new RTADCException();
		}

		public void AddError(ICollection c)
		{
			if(errors == null) errors = new ArrayList();
			errors.AddRange(c);
			if(StopOnError) throw new RTADCException();
		}

		public void AddDAUsageError(string err)
		{
			if(errors == null) errors = new ArrayList();
			errors.Add(err + " @ " + GetLocation());
			if(StopOnError || StopOnDAUsageError)
				throw new RTADCException();
		}

		public void AddWarning(string err)
		{
			if(warnings == null) warnings = new ArrayList();
			warnings.Add(err);
		}

		// ---

		public bool StopOnError
		{
			get { return stopOnError; }
			set { stopOnError = value; }
		}

		public bool StopOnDAUsageError
		{
			get { return stopOnDAUsageError; }
			set { stopOnDAUsageError = value; }
		}

		public void ResetErrors()
		{
			nameCtx = new ArrayList();
			errors = null;
			warnings = null;
		}

		public bool HasErrors()
		{
			return ((errors != null) && (errors.Count > 0));
		}

		public bool HasWarnings()
		{
			return ((warnings != null) && (warnings.Count > 0));
		}

		public ArrayList GetErrors()
		{
			return errors;
		}

		public ArrayList GetWarnings()
		{
			return warnings;
		}

		public string GetLocation()
		{
			return DependencyUtils.Array2String(
				DependencyUtils.ArrayList2Array(nameCtx), "->");
		}

	} // EOC

	/// <summary>
	/// custom expetion class to distingush this exception
	/// </summary>
	public class RTADCException : Exception
	{
		
		public RTADCException() : base("Run-time attribute dependency check error")
		{
		}
		
		public RTADCException(string s) : base(s)
		{
		
		}
	} // EOC
}
