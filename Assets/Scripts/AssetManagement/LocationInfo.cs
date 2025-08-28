namespace WebGLCD
{
public readonly struct LocationInfo
{
    public readonly string PrimaryKey;
    public readonly string InternalId;
    public readonly ResourceLocationType Source;
    public readonly long Size;

    public LocationInfo(string primaryKey, string internalId, ResourceLocationType source, long size)
    {
        PrimaryKey = primaryKey;
        InternalId = internalId;
        Source = source;
        Size = size;
    }

    public override string ToString()
    {
        return $"- PrimaryKey: {PrimaryKey}\n"
            + $"  InternalId: {InternalId}\n"
            + $"  Source: {Source}\n"
            + $"  Size: {Size / 1024:F2}KB";
    }
}
}