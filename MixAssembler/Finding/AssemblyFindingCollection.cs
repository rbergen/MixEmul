using MixLib.Misc;
using System.Collections.ObjectModel;
using System.Linq;

namespace MixAssembler.Finding
{
	public class AssemblyFindingCollection : Collection<AssemblyFinding>
	{
		public bool ContainsDebugs 
			=> Contains(Severity.Debug);

		public bool ContainsErrors 
			=> Contains(Severity.Error);

		public bool ContainsInfos 
			=> Contains(Severity.Info);

		public bool ContainsWarnings 
			=> Contains(Severity.Warning);

		bool Contains(Severity severity)
			=> Items.Any(finding => finding.Severity == severity);
	}
}
