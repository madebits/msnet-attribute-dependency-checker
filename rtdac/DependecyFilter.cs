using System;
using System.Reflection;

namespace rtadc
{
	
	public interface IDependecyFilter
	{
		bool CanProcess(object t);
	}

	public class DefaultDependecyFilter : IDependecyFilter
	{
		public DefaultDependecyFilter()
		{
		}

		public virtual bool CanProcess(object t)
		{
			return true;
		}
	} //EOC

	/// <summary>
	/// very simple filter to demostrate the idea
	/// only the specified classes will be checked
	/// </summary>
	public class ClassDependecyFilter : DefaultDependecyFilter
	{
		private Type[] conly = null;
		private string[] csonly = null;

		// simple counter
		private bool enableCounter = false;
		private long assemblies = 0L;
		private long classes = 0L;
		private long methods = 0L;

		public ClassDependecyFilter(){}

		public Type[] ProcessTheseClassesOnly
		{
			get { return conly; }
			set { conly = value; }
		}

		public string[] ProcessTheseClassesNamesOnly
		{
			get { return csonly; }
			set { csonly = value; }
		}
		
		public override bool CanProcess(object t)
		{
			if(t is Assembly) assemblies++;
			if(t is MethodInfo) methods++;
            if((t is Type) && ((Type)t).IsClass)
			{
				classes++;
				// simple linear search
				Type tt = (Type)t;
				if(conly != null)
				{
					for(int i = 0; i < conly.Length; i++)
					{
						if(conly[i].Equals(tt))
							return true;
					}
				}
				if(csonly != null)
				{
					for(int i = 0; i < csonly.Length; i++)
					{
						if(csonly[i].EndsWith(tt.Name))
							return true;
					}
				}
				return ((conly == null) && (csonly == null)) ? true : false;
			}
			return base.CanProcess(t);
		}

		// --- counter

		public bool EnableCounters
		{
			get { return enableCounter; }
			set { enableCounter = value; }
		}

		public long[] GetCounters()
		{
			return new long[]{assemblies, classes, methods};
		}

		public void ResetCounters()
		{
			assemblies = classes = methods = 0L;
		}

	} // EOC
}
