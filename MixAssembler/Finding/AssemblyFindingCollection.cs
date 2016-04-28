using System.Collections.ObjectModel;
using MixLib.Misc;

namespace MixAssembler.Finding
{
	public class AssemblyFindingCollection : Collection<AssemblyFinding>
	{
        public bool ContainsDebugs => contains(Severity.Debug);

        public bool ContainsErrors => contains(Severity.Error);

        public bool ContainsInfos => contains(Severity.Info);

        public bool ContainsWarnings => contains(Severity.Warning);

        bool contains(Severity severity)
        {
            foreach (AssemblyFinding finding in Items)
            {
                if (finding.Severity == severity) return true;
            }
            return false;
        }
    }
}
