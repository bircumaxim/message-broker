namespace Domain.Infrastructure.Mapping
{
    public interface IMapper<in TM, out TP>
    {
        TP Map(TM model);
    }
}