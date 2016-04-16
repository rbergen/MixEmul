using System.Collections.ObjectModel;
using MixLib.Misc;

namespace MixAssembler.Finding
{
	public class AssemblyFindingCollection : Collection<AssemblyFinding>
	{
		private bool contains(Severity severity)
		{
			foreach (AssemblyFinding finding in base.Items)
			{
				if (finding.Severity == severity)
				{
					return true;
				}
			}
			return false;
		}

		public bool ContainsDebugs
		{
			get
			{
				return contains(Severity.Debug);
			}
		}

		public bool ContainsErrors
		{
			get
			{
				return contains(Severity.Error);
			}
		}

		public bool ContainsInfos
		{
			get
			{
				return contains(Severity.Info);
			}
		}

		public bool ContainsWarnings
		{
			get
			{
				return contains(Severity.Warning);
			}
		}
	}
}
