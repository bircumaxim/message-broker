using System.Collections.Generic;

namespace Domain.Infrastructure.Mapping
{
    public static class ListMapper
    {
        public static List<TR> Map<T, TR> (List<T> list, IMapper<T, TR> mapFunction) {
            var result = new List<TR>();
            list?.ForEach(item => result.Add(mapFunction.Map(item)));
            return result;
        }
    }
}