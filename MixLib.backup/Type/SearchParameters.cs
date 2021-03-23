
namespace MixLib.Type
{
	public class SearchParameters
	{
		public string SearchText { get; set; }
		public FieldTypes SearchFields { get; set; }
		public int? SearchFromWordIndex { get; set; }
		public FieldTypes SearchFromField { get; set; }
		public int SearchFromFieldIndex { get; set; }
		public bool MatchWholeWord { get; set; }
		public bool WrapSearch { get; set; }
		public bool IncludeOriginalSource { get; set; }

		public void UpdateWithResult(SearchResult result)
		{
			SearchFromWordIndex = result.WordIndex;
			SearchFromField = result.Field;
			SearchFromFieldIndex = result.FieldIndex + 1;
		}
	}
}
