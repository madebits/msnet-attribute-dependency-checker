using System;
using System.Collections;
using System.Reflection;

namespace rtadc
{
	public class RTADCClass : RTAttributeDependencyChecker
	{
		public override void Check(object t)
		{
			if(!(t is Type))
				throw new Exception("Not a type: " + t.ToString());
			if(!((Type)t).IsClass)
				throw new Exception("Not a class: " + t.ToString());
			ArrayList ctx = InitialContext;
			CheckETypeDependencies(ref ctx, t,
				RTIContextMap.ASTElementType.CLASS);

		}

		protected override void ProcessEnterElement(object t)
		{
			Type c = (Type)t;
			errors.EnterContext(c.FullName);
		}

		protected override void ProcessSubElements(ref ArrayList ctx, object t)
		{
			MethodInfo[] m = ((Type)t).GetMethods(
				BindingFlags.Instance |
				BindingFlags.Public |
				BindingFlags.DeclaredOnly |
				BindingFlags.NonPublic);

			foreach(MethodInfo mi in m)
			{
				RTADCMethod adc = new RTADCMethod();
				CopyStateTo(adc);
				adc.InitialContext = ctx;
				adc.Check(mi);
			}
		}

	}//EOC
}
