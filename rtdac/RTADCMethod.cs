using System;
using System.Collections;
using System.Reflection;

namespace rtadc
{
	public class RTADCMethod : RTAttributeDependencyChecker
	{
		public override void Check(object t)
		{
			if(!(t is MethodInfo))
				throw new Exception("Not a method info: " + t.ToString());
			ArrayList ctx = InitialContext;
			CheckETypeDependencies(ref ctx, t,
				RTIContextMap.ASTElementType.METHOD);
		}

		protected override void ProcessEnterElement(object t)
		{
			MethodInfo m = (MethodInfo)t;
			errors.EnterContext("<" + m.ToString() + ">");
		}

		protected override void ProcessSubElements(ref ArrayList ctx, object t)
		{
			// no sub elements defined
		}

	}//EOC
}
