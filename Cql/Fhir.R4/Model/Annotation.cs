using Hl7.Cql.Poco.Fhir;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Hl7.Cql.Poco.Fhir.R4.Model
{
    [FhirUri("http://hl7.org/fhir/StructureDefinition/Annotation")]
	public partial class Annotation : Element
	{

		[Choice(typeof(Reference))]
		[Choice(typeof(StringElement))]
		[JsonIgnore]
		public Element author { get; set; }
		public Reference authorReference { get => ChoiceAttribute.AsExactly<Reference>(author); set { author = value; } }
		public StringElement authorString { get => ChoiceAttribute.AsExactly<StringElement>(author); set { author = value; } }

		public DateTimeElement time { get; set; }

		[NotNull]
		public MarkdownElement text { get; set; }
	}
}
