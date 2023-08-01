﻿using Hl7.Cql.Comparers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hl7.Fhir.Model;
using Hl7.Cql.Firely.Comparers;

namespace Hl7.Cql.Firely
{
    public static class CqlComparersExtensions
    {
        public static CqlComparers AddFirelyComparers(this CqlComparers comparers)
        {
            comparers.Register(typeof(Canonical), new IValueComparer<string?>());
            comparers.Register(typeof(Code), new IValueComparer<string?>());
            comparers.Register(typeof(Date), new IValueComparer<string?>());
            comparers.Register(typeof(FhirBoolean), new IValueComparer<bool?>());
            comparers.Register(typeof(FhirDateTime), new IValueComparer<string?>());
            comparers.Register(typeof(FhirDecimal), new IValueComparer<decimal?>());
            comparers.Register(typeof(FhirString), new IValueComparer<string?>());
            comparers.Register(typeof(FhirUri), new IValueComparer<string?>());
            comparers.Register(typeof(FhirUrl), new IValueComparer<string?>());
            comparers.Register(typeof(Id), new IValueComparer<string>());
            comparers.Register(typeof(Integer), new IValueComparer<int?>());
            comparers.Register(typeof(Instant), new IValueComparer<DateTimeOffset?>());
            comparers.Register(typeof(Markdown), new IValueComparer<string?>());
            comparers.Register(typeof(Oid), new IValueComparer<string?>());
            comparers.Register(typeof(PositiveInt), new IValueComparer<int?>());
            comparers.Register(typeof(Time), new IValueComparer<string?>());
            comparers.Register(typeof(UnsignedInt), new IValueComparer<int?>());
            comparers.Register(typeof(Uuid), new IValueComparer<string?>());
            comparers.Register(typeof(Identifier), new IdentifierComparer(comparers, comparers));

            comparers.Register(typeof(Code<>), (type,_comparers) =>
            {
                var codeType = type.GetGenericArguments()[0];
                var comparerType = typeof(CodeComparer<>).MakeGenericType(codeType);
                var comparer = (ICqlComparer)Activator.CreateInstance(comparerType, _comparers)!;
                return comparer;
            });

            return comparers;
        }

        /// <summary>
        /// Adds comparers for all types derived from <see cref="Resource"/> which compare them by their <see cref="Resource.Id"/> property only.
        /// </summary>
        /// <remarks>
        /// CQL models all complex types such as FHIR resources as Tuple types.  Equality semantics for Tuples states that each property must be compared
        /// and equal for two Tuple instances to be considered equal.  For FHIR resources with their large numbers of properties, these comparisons
        /// are expensive and usually superfluous.  Comparing by resource ID alone is sufficient in nearly every use case.
        /// </remarks>
        /// <see href="https://cql.hl7.org/09-b-cqlreference.html#equal">
        /// <param name="comparers"></param>
        /// <param name="idComparer"></param>
        /// <returns></returns>
        public static CqlComparers CompareResourcesById(this CqlComparers comparers, StringComparer idComparer)
        {
            var derviedFromResource = typeof(Patient).Assembly.GetTypes()
                .Where(t => typeof(Resource).IsAssignableFrom(t));
            var resourceIdComparer = new ResourceIdCqlComparer(new StringCqlComparer(idComparer));
            foreach(var type in derviedFromResource)
            {
                comparers.Register(type, resourceIdComparer);
            }
            return comparers;
        }
    }
}
