using System;

namespace rtadc
{

	public class AttributeContextSlot
	{
		private object[] attributes = null;
		private DependencyAttribute dependencies = null;

		public AttributeContextSlot()
		{
		}

		public object[] Attributes
		{
			get { return attributes; }
			set { attributes = value; }
		}

		public DependencyAttribute Dependencies
		{
			get { return dependencies; }
			set { dependencies = value; }
		}

		public override string ToString()
		{
			string s = "Attributes: <";
			if(attributes != null)
				s += DependencyUtils.Array2String(attributes, null);
				s += ">";
			if(dependencies != null)
			{
				s += " Dependencies: <" + dependencies.ToString() + ">";
			}
			return s;
		}
	}
}
