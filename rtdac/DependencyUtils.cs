using System;
using System.Collections;
using System.Text;

namespace rtadc
{

	public class DependencyUtils
	{
		private DependencyUtils()
		{
		}

		public static Type[] GetRequireDisallowConflicts(Type[] r, Type[] d)
		{
			if((r == null) || (d == null)) return null;
			Set s1 = Array2Set(r);
			Set s2 = Array2Set(d);
			Set s = s1 & s2;
			if(s.Count != 0)
			{
				return Set2TypeArray(s);
			}
			return null;
		}

		public static Set Array2Set(object[] t)
		{
			if(t == null) return null;
			Set s = new Set(t.Length);
			for(int i = 0; i < t.Length; i++)
			{
				s.Add(t[i]);	
			}
			return s;
		}

		public static Type[] Set2TypeArray(Set s)
		{
			if(s == null) return null;
			Type[] t = new Type[s.Keys.Count];
			s.Keys.CopyTo(t, 0);
			return t;
		}

		public static Type[] UnionMergeTypes(Type[] t1, Type[] t2)
		{
			if(t1 == null) return t2;
			if(t2 == null) return t1;
			Set s1 = DependencyUtils.Array2Set(t1);
			Set s2 = DependencyUtils.Array2Set(t2);
			return (Type[])DependencyUtils.Set2TypeArray(
				s1.Union(s2));
		}

		public static void RemoveLastElement(ref ArrayList ctx)
		{
			if(ctx == null) ctx = new ArrayList();
			if(ctx.Count == 0) return;
			ctx.RemoveAt(ctx.Count - 1);
		}

		public static object[] ArrayList2Array(ArrayList a)
		{
			if(a == null) return null;
			object[] o = new object[a.Count];
			a.CopyTo(0, o, 0, a.Count);
			return o;
		}
		
		public static bool AddRDError(
			ref ErrorReport errors, Type[] o, string prefix)
		{
			if(o != null)
			{ 
				if(errors != null)
					errors.AddError("Require-Disallow conflicts <"
						+ prefix + ">: "
						+ DependencyUtils.Array2String(o, null));
				return true;
			}
			return false;
		}

		public static string Array2String(object[] o, string sep)
		{
			return DependencyAttribute.Array2String(o, sep);
		}

	} //EOC
}
