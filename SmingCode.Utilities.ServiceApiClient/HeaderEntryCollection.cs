using System.Collections;
using System.Net.Http.Headers;

namespace SmingCode.Utilities.ServiceApiClient;

public class HeaderEntryCollection : IEnumerable<KeyValuePair<string, IEnumerable<string>>>
{
    internal List<KeyValuePair<string, IEnumerable<string>>> Headers { get; } = [];

    public void Add(
        string key,
        string value
    ) => Headers.Add(new(key, [ value ]));

    public void Add(
        string key,
        IEnumerable<string> values
    ) => Headers.Add(new(key, values));

    public void Add(
        string key,
        params string[] values
    ) => Headers.Add(new(key, values));

    public void Add(
        IEnumerable<KeyValuePair<string, string>> entries
    ) => entries.ToList().ForEach(entry => Headers.Add(new(entry.Key, [ entry.Value ])));

    public void Add(
        IEnumerable<KeyValuePair<string, IEnumerable<string>>> entries
    ) => entries.ToList().ForEach(entry => Headers.Add(new(entry.Key, entry.Value)));

    public void Add(
        HttpHeaders existingHeaders
    ) => existingHeaders.ToList().ForEach(existingHeader => Headers.Add(new(existingHeader.Key, existingHeader.Value)));

    public IEnumerator<KeyValuePair<string, IEnumerable<string>>> GetEnumerator()
        => Headers.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}