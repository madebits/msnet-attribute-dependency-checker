using System;

namespace rtadc
{
	// --- these (static) methods rely on the DependencyAttribute implementation
	// reflection can be used here to write code that does not depend on
	// the specific Require*, Dissallow* property names, using the RTIContextMap

	public class RTIUtils
	{
		private RTIUtils()
		{
		}

		public static bool HasRequireDisallowConflicts(DependencyAttribute da, ref ErrorReport errors)
		{
			try
			{
				da = ValidateArguments(da);
			} 
			catch(Exception ex)
			{
				errors.AddDAUsageError(ex.Message);
			}
			bool result = false;
			Type[] r1 = DependencyUtils.GetRequireDisallowConflicts(da.RequiredAssemblyAttributes, da.DisallowedAssemblyAttributes);
			Type[] r2 = DependencyUtils.GetRequireDisallowConflicts(da.RequiredClassAttributes, da.DisallowedClassAttributes);
			Type[] r3 = DependencyUtils.GetRequireDisallowConflicts(da.RequiredMethodAttributes, da.DisallowedMethodAttributes);
			result |= DependencyUtils.AddRDError(ref errors, r1, "Assembly");
			result |= DependencyUtils.AddRDError(ref errors, r1, "Class");
			result |= DependencyUtils.AddRDError(ref errors, r1, "Method");
			return result;
		}

		public static DependencyAttribute UnionMerge(DependencyAttribute da1, DependencyAttribute da2)
		{
			if(da1 == null) return da2;
			if(da2 == null) return da1;
			DependencyAttribute da = new DependencyAttribute();
			da.RequiredAssemblyAttributes = DependencyUtils.UnionMergeTypes(da1.RequiredAssemblyAttributes, da2.RequiredAssemblyAttributes);
			da.DisallowedAssemblyAttributes = DependencyUtils.UnionMergeTypes(da1.DisallowedAssemblyAttributes, da2.DisallowedAssemblyAttributes);
			da.RequiredClassAttributes = DependencyUtils.UnionMergeTypes(da1.RequiredClassAttributes, da2.RequiredClassAttributes);
			da.DisallowedClassAttributes = DependencyUtils.UnionMergeTypes(da1.DisallowedClassAttributes, da2.DisallowedClassAttributes);
			da.RequiredMethodAttributes = DependencyUtils.UnionMergeTypes(da1.RequiredMethodAttributes, da2.RequiredMethodAttributes);
			da.DisallowedMethodAttributes = DependencyUtils.UnionMergeTypes(da1.DisallowedMethodAttributes, da2.DisallowedMethodAttributes);
			return da;
		}

		public static DependencyAttribute ValidateArguments(DependencyAttribute da)
		{
			da.RequiredAssemblyAttributes = DependencyAttribute.CheckArguments(da.RequiredAssemblyAttributes, AttributeTargets.Assembly);
			da.DisallowedAssemblyAttributes = DependencyAttribute.CheckArguments(da.DisallowedAssemblyAttributes, AttributeTargets.Assembly);
			da.RequiredClassAttributes = DependencyAttribute.CheckArguments(da.RequiredClassAttributes, AttributeTargets.Class);
			da.DisallowedClassAttributes = DependencyAttribute.CheckArguments(da.DisallowedClassAttributes, AttributeTargets.Class);
			da.RequiredMethodAttributes = DependencyAttribute.CheckArguments(da.RequiredMethodAttributes, AttributeTargets.Method);
			da.DisallowedMethodAttributes = DependencyAttribute.CheckArguments(da.DisallowedMethodAttributes, AttributeTargets.Method);
			return da;
		}

	}//EOC
}
