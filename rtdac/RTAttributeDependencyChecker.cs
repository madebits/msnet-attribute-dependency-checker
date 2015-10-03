using System;
using System.Collections;
using System.Reflection;

namespace rtadc
{

	public abstract class RTAttributeDependencyChecker
	{
		public RTAttributeDependencyChecker()
		{
		}

		// state
		private ICheckLogger log = null;
		private bool processInheritedAttributes = true;
		private IDependecyFilter filter = new DefaultDependecyFilter();
		public ErrorReport errors = new ErrorReport();
		private ArrayList initialContext = null;
		// used for log pretty print
		private bool logFirstCheckETypeDependencies = false;

		/// <summary>
		/// copies this config/check state to another
		/// RTAttributeDependencyChecker object
		/// </summary>
		public void CopyStateTo(RTAttributeDependencyChecker rtc)
		{
			rtc.Logger = Logger;
			rtc.Filter = Filter;
			rtc.ProcessInheritedAttributes
				= ProcessInheritedAttributes;
			rtc.errors = errors;
			rtc.InitialContext = InitialContext;
			rtc.logFirstCheckETypeDependencies
				= logFirstCheckETypeDependencies;
		}

		/// <summary>
		/// use if you want to reuse a RTAttributeDependencyChecker
		/// object to check another element of the same type 
		/// </summary>
		public void ResetCheckState()
		{
			initialContext = null;
			errors.ResetErrors();
		}

		// ---

		public ICheckLogger Logger
		{
			get { return log; }
			set { this.log = value; }
		}

		public IDependecyFilter Filter
		{
			get
			{
				return filter;
			}
			set
			{
				if(value != null)
					this.filter = value;
			}
		}

		public bool ProcessInheritedAttributes
		{
			get{ return processInheritedAttributes; }
			set{ processInheritedAttributes = value;}
		}

		/// <summary>
		/// not to be used directly
		/// </summary>
		public ArrayList InitialContext
		{
			get
			{ 
				if(initialContext == null)
					initialContext = new ArrayList();
				return initialContext;
			}
			set 
			{
				initialContext = value;
			}
		}

		// ---

		public abstract void Check(object t);

		protected abstract void ProcessEnterElement(object t);

		protected abstract void ProcessSubElements(
			ref ArrayList ctx, object t);
		
		// ---

		protected void CheckETypeDependencies(
			ref ArrayList ctx,
			object t,
			RTIContextMap.ASTElementType type)
		{
			if(!filter.CanProcess(t)) return;
			LogCheckETypeDependencies(ctx, t, type);
			try
			{
				ProcessEnterElement(t);
			} 
			catch (Exception ex)
			{
				errors.AddError(ex.Message);
			}
			object[] attribList = GetTypeAttributes(ref ctx,
				(ICustomAttributeProvider)t, type);
			if(!logFirstCheckETypeDependencies)
			{
				LogCheckETypeDependencies(ctx, t, type);
				logFirstCheckETypeDependencies = true;
			}
			try
			{
				ProcessSubElements(ref ctx, t);
			}
			catch (Exception ex)
			{
				errors.AddError(ex.Message);
			}
			CheckConstrains(ctx, attribList, type);
			DependencyUtils.RemoveLastElement(ref ctx);
			errors.LeaveContext();
		}
		
		private void LogCheckETypeDependencies(
			ArrayList ctx,
			object t,
			RTIContextMap.ASTElementType type)
		{
			if((ctx != null) && (ctx.Count > 0))
			{
				string s = string.Empty;
				int i = ctx.Count;
				if(!logFirstCheckETypeDependencies) i--;
				if(i > 0)
				{
					s = new String('\t', i);
				}
				AttributeContextSlot acs =
					(AttributeContextSlot)ctx[ctx.Count - 1];
				Log("->" + s + "[" + type.ToString()
					+ "] {" + t.ToString() + "} " + acs.ToString());
			}
		}
			
		// -------

		/// <summary>
		/// get attributes for type and updates parent depends in the context
		/// adds this element attribute info to the context
		/// </summary>
		/// <param name="ctx">context</param>
		/// <param name="t">program element</param>
		/// <param name="type">internal type</param>
		/// <returns></returns>
		private object[] GetTypeAttributes(
			ref ArrayList ctx,
			ICustomAttributeProvider t,
			RTIContextMap.ASTElementType type)
		{
			object[] attribList =
				t.GetCustomAttributes(ProcessInheritedAttributes);
			AttributeContextSlot acs = BuildAttribCtx(
				ref ctx, attribList, type);
			ctx.Add(acs);
			return attribList;
		}

		/// <summary>
		/// scan the attributes and updates the parent context
		/// </summary>
		/// <param name="ctx"></param>
		/// <param name="attribList"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		private AttributeContextSlot BuildAttribCtx(
			ref ArrayList ctx,
			object[] attribList,
			RTIContextMap.ASTElementType type)
		{
			AttributeContextSlot acs = new AttributeContextSlot();
			if(attribList != null)
			{
				DependencyAttribute cda = new DependencyAttribute();
				foreach(Attribute a in attribList)
				{
					object[] da = a.GetType().GetCustomAttributes(
						typeof(DependencyAttribute),
						ProcessInheritedAttributes);
					if(da != null && da.Length > 0)
					{
						DependencyAttribute d = (DependencyAttribute)da[0];
						if(RTIUtils.HasRequireDisallowConflicts(d, ref errors))
							return acs;
						DependencyAttribute temp =
							ProcessDependencyAttribute(ref ctx, d, type);
						cda = RTIUtils.UnionMerge(temp, cda);
					}
				}
				if(RTIUtils.HasRequireDisallowConflicts(cda, ref errors))
					return acs;
				acs.Attributes = attribList;
				acs.Dependencies = cda;
			}
			return acs;
		}

		/// <summary>
		/// process a single depends attrib, updates the parent context
		/// </summary>
		/// <param name="ctx"></param>
		/// <param name="da"></param>
		/// <param name="e"></param>
		/// <returns></returns>
		// note this code can be optimized not to use reflection
		// but to be generated by using reflection over DependencyAttribute
		// and then compiled with the rest of code
		private DependencyAttribute ProcessDependencyAttribute(
			ref ArrayList ctx,
			DependencyAttribute da,
			RTIContextMap.ASTElementType e)
		{
			if(RTIUtils.HasRequireDisallowConflicts(da, ref errors))
				return null;
			DependencyAttribute newDA = new DependencyAttribute();
			Type dat = typeof(DependencyAttribute);
			PropertyInfo[] dap = dat.GetProperties();
			foreach(PropertyInfo dapi in dap)
			{
				if(!dapi.PropertyType.Equals((new Type[0]).GetType()))
					continue;
				Type[] tp = (Type[])dapi.GetValue(da, null);
				if(tp == null) continue;
				// process the custom property value
				int i = RTIContextMap.Index(e, dapi.Name);
				if(i == RTIContextMap.UNKNOWN)
				{
					errors.AddWarning("Dependecy reference to unknown/unpropper element");
					continue;
				}
				if(i >= 0)
				{
					if(i >= ctx.Count)
					{
						errors.AddWarning("Referenced (grand)parent is not in context");
						continue;
					}
					AttributeContextSlot acs
						= (AttributeContextSlot)ctx[i];
					DependencyAttribute cda = acs.Dependencies;
					if(cda == null)
						cda = new DependencyAttribute();
					tp = DependencyUtils.UnionMergeTypes(tp,
						(Type[])dapi.GetValue(cda, null));
					dapi.SetValue(cda, tp, null);
					acs.Dependencies = cda;
					ctx[i] = acs;
				}
				else // ContextMap.CURRENT, or sub element
				{
					dapi.SetValue(newDA, tp, null);
				}
			}
			return newDA;
		}

		/// <summary>
		/// check the depends constrains for a type, using synthesized context information
		/// </summary>
		/// <param name="ctx"></param>
		/// <param name="attribList"></param>
		/// <param name="type"></param>
		// check constrains for ctx[0] to ctx[ctx.Count-1] = cda,
		// Union RM, DM for all ctx and check against attribList
		private void CheckConstrains(
			ArrayList ctx,
			object[] attribList,
			RTIContextMap.ASTElementType type)
		{
			Type dat = typeof(DependencyAttribute);
			PropertyInfo[] dap = dat.GetProperties();
			Type[] required = null;
			Type[] disallowed = null;
			for(int i = 0; i < ctx.Count; i++)
			{
				
				AttributeContextSlot acs = null;
				try
				{
					acs = (AttributeContextSlot)ctx[i];
				}
				catch(Exception ex)
				{
					string s = ex.Message;
				}
				if(acs == null) continue;
				DependencyAttribute da = acs.Dependencies;
				if(da == null) continue;
				foreach(PropertyInfo dapi in dap)
				{
					if(dapi.Name.EndsWith(
						RTIContextMap.TypePropName(type, true)))
					{
						Type[] tp = (Type[])dapi.GetValue(da, null);
						required = DependencyUtils.UnionMergeTypes(
							required, tp);
					}
					if(dapi.Name.EndsWith(
						RTIContextMap.TypePropName(type, false)))
					{
						Type[] tp = (Type[])dapi.GetValue(da, null);
						disallowed = DependencyUtils.UnionMergeTypes(
							disallowed, tp);
					}
				}
			}
			CheckRequiredDisallowed(attribList, required, disallowed, type);
		}

		/// <summary>
		/// cheks the dependens based on synthesized required-dissallow sets
		/// </summary>
		/// <param name="attribList"></param>
		/// <param name="required"></param>
		/// <param name="disallowed"></param>
		private void CheckRequiredDisallowed(
			object[] attribList,
			Type[] required,
			Type[] disallowed,
			RTIContextMap.ASTElementType type)
		{
			Type[] rd = DependencyUtils.GetRequireDisallowConflicts(
				required,
				disallowed);
			if(DependencyUtils.AddRDError(ref errors, rd, "*"))
				return;
			rd = null;
			//Set s = DependencyUtils.Array2Set(attribList);
			Set s = null;
			if(attribList != null)
			{
				s = new Set(attribList.Length);
				for(int i = 0; i < attribList.Length; i++)
				{
					string item = attribList[i].GetType().ToString();
					if(!s.Contains(item))
						s.Add(item);
				}
			}
			if(
				((s == null) && (required != null))
				||
				((s != null) && (s.Count == 0) &&
				(required != null) && (required.Length > 0))
				)
			{
				errors.AddError("Required " + type.ToString()
					+ " attribute(s) missing: "
					+ DependencyUtils.Array2String(required, " | "));
				return;
			}
			if(required != null)
			{
				for(int i = 0; i < required.Length; i++)
				{
					if(!s.Contains(required[i].ToString()))
					{
						errors.AddError("Required "
							+ type.ToString() + " attribute missing: "
							+ required[i].ToString());
					}
				}
			}
			if(disallowed != null)
			{
				for(int i = 0; i < disallowed.Length; i++)
				{
					if(s.Contains(disallowed[i].ToString()))
					{
						errors.AddError("Disallowed "
							+ type.ToString() + " attribute present: "
							+ disallowed[i].ToString());
					}
				}
			}
		}

		private void Log(string s)
		{
			if(log != null) log.Log(s);
		}

	}//EOC
}

