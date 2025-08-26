namespace WebGLCD
{
public abstract class ManagedResource
{
    public readonly string Key;

    public ResourceState State { get; protected set; }

    public int Count { get; protected set; }

    protected ManagedResource(string key) { Key = key; }
}
}