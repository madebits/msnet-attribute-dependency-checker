using System;

namespace rtadc
{
	/// <summary>
	/// This class is self contained. It can be linked alone
	/// with any project that uses the DependencyAttribute.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class DependencyAttribute : System.Attribute
	{
		private Type[] requiredAssemblyAttributes = null;
		private Type[] disallowedAssemblyAttributes = null;
		private Type[] requiredClassAttributes = null;
		private Type[] disallowedClassAttributes = null;
		private Type[] requiredMethodAttributes = null;
		private Type[] disallowedMethodAttributes = null;
		
		public DependencyAttribute()
		{
		}

		public Type[] RequiredAssemblyAttributes
		{
			get { return requiredAssemblyAttributes; }
			// will compile but will not work
			// so we have to postpone the check at run-time
			// requiredAssemblyAttributes = CheckArguments(value, AttributeTargets.Assembly | AttributeTargets.All);
			set { requiredAssemblyAttributes = value; }
		}

		public Type[] DisallowedAssemblyAttributes
		{
			get { return disallowedAssemblyAttributes; }
			set { disallowedAssemblyAttributes = value; }
		}

		public Type[] RequiredClassAttributes
		{
			get { return requiredClassAttributes; }
			set { requiredClassAttributes = value; }
		}

		public Type[] DisallowedClassAttributes
		{
			get { return disallowedClassAttributes; }
			set { disallowedClassAttributes = value; }
		}

		public Type[] RequiredMethodAttributes
		{
			get { return requiredMethodAttributes; }
			set { requiredMethodAttributes = value; }
		}

		public Type[] DisallowedMethodAttributes
		{
			get { return disallowedMethodAttributes; }
			set { disallowedMethodAttributes = value; }
		}
	
		public override string ToString()
		{
			string s = string.Empty;
			string sep = ", ", sep2 = " ";
			Type[] t = null;
			t = RequiredAssemblyAttributes;
			if(t != null) s += "RA " + Array2String(t, sep) + sep2;
			t = DisallowedAssemblyAttributes;
			if(t != null) s += "DA " + Array2String(t, sep) + sep2;
			t = RequiredClassAttributes;
			if(t != null) s += "RC " + Array2String(t, sep) + sep2;
			t = DisallowedClassAttributes;
			if(t != null) s += "DC " + Array2String(t, sep) + sep2;
			t = RequiredMethodAttributes;
			if(t != null) s += "RM " + Array2String(t, sep) + sep2;
			t = DisallowedMethodAttributes;
			if(t != null) s += "DM " + Array2String(t, sep) + sep2;
			return s;
		}

		/// <summary>
		/// utility method, placed here to make DependencyAttribute
		/// self contained, so it can be distributed/used alone as a class
		/// </summary>
		public static string Array2String(object[] o, string sep)
		{
			if((o == null) || (o.Length == 0)) return string.Empty;
			if(sep == null) sep = " ";
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			for(int i = 0; i < o.Length; i++)
			{
				sb.Append(o[i].ToString());
				if(i < (o.Length - 1))
					sb.Append(sep);
			}
			return sb.ToString();
		}

		// ---

		/// <summary>
		/// we require that an attribute usage for example in RequiredClassAttributes property array
		/// to have an AttributeUsage AttributeTarget.Class bit, and so on
		/// we also use a set membership here, an attribute cannot be specfied
		/// twice in a property Type[] array
		/// </summary>
		/// <param name="t"></param>
		/// <param name="validOn"></param>
		/// <returns>the input Type[] array without repetititon entries</returns>
		public static Type[] CheckArguments(
			Type[] t, AttributeTargets validOn)
		{
			if(t == null || t.Length == 0) return null;
			System.Collections.Hashtable h =
				new System.Collections.Hashtable(t.Length);
            for(int i = 0; i < t.Length; i++)
			{
				if(!t[i].IsSubclassOf(typeof(System.Attribute)))
				{
					throw new Exception("DependencyAttribute: Non attribute type passed as parament: " + t[i].ToString());
				}
				object[] attribList =
					t[i].GetCustomAttributes(
					typeof(System.AttributeUsageAttribute), true);
				for(int j = 0; j < attribList.Length; j++)
				{
					System.AttributeUsageAttribute au =
						(System.AttributeUsageAttribute)attribList[j];
					//System.Diagnostics.Debug.WriteLine("--  " + j + "  " + au.ValidOn);
					if(!((au.ValidOn & validOn) == validOn))
					{
						throw new Exception("DependencyAttribute: AttributeTargets do not match parameter: <" + au.ValidOn + " != " + validOn + "> " + t[i].ToString());
					}
				}
				if(!h.ContainsKey(t[i]))
				{
					h.Add(t[i], null);
				}
			}
			Type[] newT = new Type[h.Count];
			h.Keys.CopyTo(newT, 0);
			return newT;
		}

	}//EOC
}
