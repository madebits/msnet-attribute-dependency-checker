using System;

namespace rtadc
{

	public abstract class RTIContextMap
	{
		public enum ASTElementType {ASSEMBLY, CLASS, METHOD}
		public static readonly int UNKNOWN = System.Int32.MinValue;
		public static readonly int CURRENT = -1;
	
		private RTIContextMap()	
		{
		}

		/*
				A	C	M
			RA	-1	0	1
			RC	-2	-1	0
			RM	-3	-2	-1
			
			positive index for parent dep attribs
			negative "-" childen "-"
			-1 current
		*/
        public static int Index(ASTElementType t, string s)
		{
			switch(s)
			{
				case "RequiredAssemblyAttributes":
					if(t == ASTElementType.ASSEMBLY) return -1;
					if(t == ASTElementType.CLASS) return 0;
					if(t == ASTElementType.METHOD) return 1;
					break;
				case "DisallowedAssemblyAttributes":
					if(t == ASTElementType.ASSEMBLY) return -1;
					if(t == ASTElementType.CLASS) return 0;
					if(t == ASTElementType.METHOD) return 1;
					break;
				
				case "RequiredClassAttributes":
					if(t == ASTElementType.ASSEMBLY) return -2;
					if(t == ASTElementType.CLASS) return -1;
					if(t == ASTElementType.METHOD) return 0;
					break;
				case "DisallowedClassAttributes":
					if(t == ASTElementType.ASSEMBLY) return -2;
					if(t == ASTElementType.CLASS) return -1;
					if(t == ASTElementType.METHOD) return 0;
					break;
				case "RequiredMethodAttributes":
					if(t == ASTElementType.ASSEMBLY) return -3;
					if(t == ASTElementType.CLASS) return -2;
					if(t == ASTElementType.METHOD) return -1;
					break;
				case "DisallowedMethodAttributes":
					if(t == ASTElementType.ASSEMBLY) return -3;
					if(t == ASTElementType.CLASS) return -2;
					if(t == ASTElementType.METHOD) return -1;
					break;
			}
			return UNKNOWN;
		}

		public static string TypePropName(ASTElementType t, bool required)
		{
			string type = null;
			switch(t)
			{
				case ASTElementType.ASSEMBLY:
					type = "Assembly"; //ASTElementType.ASSEMBLY.ToString(); 
					break;
				case ASTElementType.CLASS:
					type = "Class";
					break;
				case ASTElementType.METHOD:
					type = "Method";
					break;
			}
			if(type == null) return null;
			return ((required) ? "Required" : "Disallowed") + type + "Attributes";
		}
	} // EOC
}
