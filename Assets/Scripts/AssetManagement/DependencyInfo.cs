namespace WebGLCD
{
public readonly struct DependencyInfo
{
    public readonly string PrimaryKey;
    public readonly string InternalId;
    public readonly AssetSource Source;
    public readonly long Size;

    public DependencyInfo(string primaryKey, string internalId, AssetSource source, long size)
    {
        PrimaryKey = primaryKey;
        InternalId = internalId;
        Source = source;
        Size = size;
    }

    public override string ToString()
    {
        return $"- Key: {PrimaryKey}\n"
            + $"  Source: {Source}\n"
            + $"  Size: {Size / 1024:F2}KB";
    }
}
}