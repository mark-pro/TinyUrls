using Riok.Mapperly.Abstractions;
using TinyUrls.Types;

namespace TinyUrls.Api.VerticalSlices;

public interface IRequestMapper {
    RetrieveTinyUrlRequest Map(ShortCodeType request);
}

[Mapper]
public partial class RequestMapper : IRequestMapper {
    [MapPropertyFromSource(nameof(RetrieveTinyUrlRequest.ShortCode))]
    public partial RetrieveTinyUrlRequest Map(ShortCodeType request);
}