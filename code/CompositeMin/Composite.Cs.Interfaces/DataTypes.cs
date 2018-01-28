// Warning: Some assembly references could not be loaded. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the references to the list of loaded assemblies.
// Composite.DataTypes
using Composite;
using Microsoft.FSharp.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[CompilationMapping(SourceConstructFlags.Module)]
public static class DataTypes
{
	[Serializable]
	[StructLayout(LayoutKind.Auto, CharSet = CharSet.Auto)]
	[DebuggerDisplay("{__DebugDisplay(),nq}")]
	[CompilationMapping(SourceConstructFlags.SumType)]
	public abstract class Composite<a> : IEquatable<Composite<a>>, IStructuralEquatable
	{
		public static class Tags
		{
			public const int Value = 0;

			public const int Composite = 1;
		}

		[Serializable]
		[DebuggerTypeProxy(typeof(Composite<>.Value@DebugTypeProxy))]
		[DebuggerDisplay("{__DebugDisplay(),nq}")]
		public class Value : Composite<a>
		{
			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			[CompilerGenerated]
			[DebuggerNonUserCode]
			internal readonly a item;

			[CompilationMapping(SourceConstructFlags.Field, 0, 0)]
			[CompilerGenerated]
			[DebuggerNonUserCode]
			public a Item
			{
				[DebuggerNonUserCode]
				get;
			}

			[CompilerGenerated]
			[DebuggerNonUserCode]
			internal Value(a item)
			{
				this.item = item;
			}
		}

		[Serializable]
		[DebuggerTypeProxy(typeof(Composite<>.Composite@DebugTypeProxy))]
		[DebuggerDisplay("{__DebugDisplay(),nq}")]
		public class Composite : Composite<a>
		{
			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			[CompilerGenerated]
			[DebuggerNonUserCode]
			internal readonly IEnumerable<Composite<a>> item;

			[CompilationMapping(SourceConstructFlags.Field, 1, 0)]
			[CompilerGenerated]
			[DebuggerNonUserCode]
			public IEnumerable<Composite<a>> Item
			{
				[DebuggerNonUserCode]
				get;
			}

			[CompilerGenerated]
			[DebuggerNonUserCode]
			internal Composite(IEnumerable<Composite<a>> item)
			{
				this.item = item;
			}
		}

		internal class Value@DebugTypeProxy
		{
			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			[CompilerGenerated]
			[DebuggerNonUserCode]
			internal Value _obj;

			[CompilationMapping(SourceConstructFlags.Field, 0, 0)]
			[CompilerGenerated]
			[DebuggerNonUserCode]
			public a Item
			{
				[CompilerGenerated]
				[DebuggerNonUserCode]
				get
				{
					return this._obj.item;
				}
			}

			[CompilerGenerated]
			[DebuggerNonUserCode]
			public Value@DebugTypeProxy(Value obj)
			{
				this._obj = obj;
			}
		}

		internal class Composite@DebugTypeProxy
		{
			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			[CompilerGenerated]
			[DebuggerNonUserCode]
			internal Composite _obj;

			[CompilationMapping(SourceConstructFlags.Field, 1, 0)]
			[CompilerGenerated]
			[DebuggerNonUserCode]
			public IEnumerable<Composite<a>> Item
			{
				[CompilerGenerated]
				[DebuggerNonUserCode]
				get
				{
					return this._obj.item;
				}
			}

			[CompilerGenerated]
			[DebuggerNonUserCode]
			public Composite@DebugTypeProxy(Composite obj)
			{
				this._obj = obj;
			}
		}

		[CompilerGenerated]
		[DebuggerNonUserCode]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public int Tag
		{
			[CompilerGenerated]
			[DebuggerNonUserCode]
			get
			{
				return (this is Composite) ? 1 : 0;
			}
		}

		[CompilerGenerated]
		[DebuggerNonUserCode]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public bool IsValue
		{
			[CompilerGenerated]
			[DebuggerNonUserCode]
			get
			{
				return this is Value;
			}
		}

		[CompilerGenerated]
		[DebuggerNonUserCode]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public bool IsComposite
		{
			[CompilerGenerated]
			[DebuggerNonUserCode]
			get
			{
				return this is Composite;
			}
		}

		[CompilerGenerated]
		[DebuggerNonUserCode]
		internal Composite()
		{
		}

		[CompilationMapping(SourceConstructFlags.UnionCase, 0)]
		public static Composite<a> NewValue(a item)
		{
			return new Value(item);
		}

		[CompilationMapping(SourceConstructFlags.UnionCase, 1)]
		public static Composite<a> NewComposite(IEnumerable<Composite<a>> item)
		{
			return new Composite(item);
		}

		[CompilerGenerated]
		[DebuggerNonUserCode]
		internal object __DebugDisplay()
		{
			return ExtraTopLevelOperators.PrintFormatToString<FSharpFunc<Composite<a>, string>>((PrintfFormat<FSharpFunc<Composite<a>, string>, Unit, string, string>)new PrintfFormat<FSharpFunc<Composite<a>, string>, Unit, string, string, string>("%+0.8A")).Invoke(this);
		}

		[CompilerGenerated]
		public override string ToString()
		{
			return ExtraTopLevelOperators.PrintFormatToString<FSharpFunc<Composite<a>, string>>((PrintfFormat<FSharpFunc<Composite<a>, string>, Unit, string, string>)new PrintfFormat<FSharpFunc<Composite<a>, string>, Unit, string, string, Composite<a>>("%+A")).Invoke(this);
		}

		[CompilerGenerated]
		public sealed override int GetHashCode(IEqualityComparer comp)
		{
			if (this != null)
			{
				int num = 0;
				if (this is Value)
				{
					Value value = (Value)this;
					num = 0;
					a item = value.item;
					return -1640531527 + (LanguagePrimitives.HashCompare.GenericHashWithComparerIntrinsic<a>(comp, item) + ((num << 6) + (num >> 2)));
				}
				Composite composite = (Composite)this;
				num = 1;
				return -1640531527 + (LanguagePrimitives.HashCompare.GenericHashWithComparerIntrinsic<IEnumerable<Composite<a>>>(comp, composite.item) + ((num << 6) + (num >> 2)));
			}
			return 0;
		}

		[CompilerGenerated]
		public sealed override int GetHashCode()
		{
			return this.GetHashCode(LanguagePrimitives.GenericEqualityComparer);
		}

		[CompilerGenerated]
		public sealed override bool Equals(object obj, IEqualityComparer comp)
		{
			if (this != null)
			{
				Composite<a> composite = obj as Composite<a>;
				if (composite != null)
				{
					Composite<a> composite2 = composite;
					int num = (this is Composite) ? 1 : 0;
					Composite<a> composite3 = composite2;
					int num2 = (composite3 is Composite) ? 1 : 0;
					if (num == num2)
					{
						if (this is Value)
						{
							Value value = (Value)this;
							Value value2 = (Value)composite2;
							a item = value.item;
							a item2 = value2.item;
							return LanguagePrimitives.HashCompare.GenericEqualityWithComparerIntrinsic<a>(comp, item, item2);
						}
						Composite composite4 = (Composite)this;
						Composite composite5 = (Composite)composite2;
						return LanguagePrimitives.HashCompare.GenericEqualityWithComparerIntrinsic<IEnumerable<Composite<a>>>(comp, composite4.item, composite5.item);
					}
					return false;
				}
				return false;
			}
			return obj == null;
		}

		[CompilerGenerated]
		public sealed override bool Equals(Composite<a> obj)
		{
			if (this != null)
			{
				if (obj != null)
				{
					int num = (this is Composite) ? 1 : 0;
					int num2 = (obj is Composite) ? 1 : 0;
					if (num == num2)
					{
						if (this is Value)
						{
							Value value = (Value)this;
							Value value2 = (Value)obj;
							a item = value.item;
							a item2 = value2.item;
							return LanguagePrimitives.HashCompare.GenericEqualityERIntrinsic<a>(item, item2);
						}
						Composite composite = (Composite)this;
						Composite composite2 = (Composite)obj;
						return LanguagePrimitives.HashCompare.GenericEqualityERIntrinsic<IEnumerable<Composite<a>>>(composite.item, composite2.item);
					}
					return false;
				}
				return false;
			}
			return obj == null;
		}

		[CompilerGenerated]
		public sealed override bool Equals(object obj)
		{
			Composite<a> composite = obj as Composite<a>;
			if (composite != null)
			{
				return this.Equals(composite);
			}
			return false;
		}
	}
}
