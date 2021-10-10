using System.Collections.Generic;

namespace Services.Interfaces
{
    public interface INgram
    {
        List<string> ToList();
        string ToSearchTerm();
    }
}
