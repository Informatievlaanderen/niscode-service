namespace NisCodeService.Abstractions
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface INisCodeFinder<T>
    {
        Task<string?> FindAsync(T id, CancellationToken ct);
    }
}
