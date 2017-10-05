namespace Domain.Infrastructure.Mapping
{
    public interface ITwoWaysMapper<T, TP> : IMapper<T, TP> {
        T InverseMap(TP model);
    }
}