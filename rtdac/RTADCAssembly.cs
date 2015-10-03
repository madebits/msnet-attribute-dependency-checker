using System;
using System.Collections;
using System.Reflection;

namespace rtadc
{
	public class RTADCAssembly : RTAttributeDependencyChecker
	{
		public override void Check(object t)
		{
			if(!(t is Assembly))
				throw new Exception("Not an assembly: " + t.ToString());
			ArrayList ctx = InitialContext;
			CheckETypeDependencies(ref ctx, t,
				RTIContextMap.ASTElementType.ASSEMBLY);
		}

		protected override void ProcessEnterElement(object t)
		{
			Assembly a = (Assembly)t;
			errors.EnterContext(a.FullName);
		}

		protected override void ProcessSubElements(ref ArrayList ctx, object t)
		{
			Type[] subt = ((Assembly)t).GetTypes();
			foreach(Type st in subt)
			{
				if(st.IsClass)
				{
					RTADCClass adc = new RTADCClass();
					CopyStateTo(adc);
					adc.InitialContext = ctx;
					adc.Check(st);
				}
			}
		}

	}//EOC
}
