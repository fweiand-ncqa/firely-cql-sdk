using Hl7.Cql.Poco.Fhir;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Hl7.Cql.Poco.Fhir.R4.Model
{
    [FhirUri("http://hl7.org/fhir/StructureDefinition/MedicinalProductIndication")]
	public partial class MedicinalProductIndication : DomainResource
	{

		public ICollection<Reference> subject { get; set; }

		public CodeableConcept diseaseSymptomProcedure { get; set; }

		public CodeableConcept diseaseStatus { get; set; }

		public ICollection<CodeableConcept> comorbidity { get; set; }

		public CodeableConcept intendedEffect { get; set; }

		public Quantity duration { get; set; }

		public ICollection<OtherTherapyComponent> otherTherapy { get; set; }

		public ICollection<Reference> undesirableEffect { get; set; }

		public ICollection<Population> population { get; set; }
		[FhirUri("http://hl7.org/fhir/StructureDefinition/MedicinalProductIndication.otherTherapy")]
		public partial class OtherTherapyComponent
		{

			public string id { get; set; }

			public ICollection<Extension> extension { get; set; }

			public ICollection<Extension> modifierExtension { get; set; }

			[NotNull]
			public CodeableConcept therapyRelationshipType { get; set; }

			[NotNull]
			[Choice(typeof(CodeableConcept))]
			[Choice(typeof(Reference))]
			[JsonIgnore]
			public Element medication { get; set; }
			public CodeableConcept medicationCodeableConcept { get => ChoiceAttribute.AsExactly<CodeableConcept>(medication); set { medication = value; } }
			public Reference medicationReference { get => ChoiceAttribute.AsExactly<Reference>(medication); set { medication = value; } }
		}
	}
}
