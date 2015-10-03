using System;
using attrib2;

namespace testattrib
{
	[RequiresMethodAttribute(typeof(Method1Attribute))]
	public class Decorated
	{
		public Decorated()
		{

		}
	}//EOC

	[AttributeUsage(AttributeTargets.Method)]
	public class Method1Attribute : System.Attribute
	{
		public Method1Attribute()
		{
		}
	} // EOC
}
