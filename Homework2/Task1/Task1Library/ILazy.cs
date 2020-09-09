/// <summary>
/// Interface for lazy instantionation of an object.
/// </summary>
/// <typeparam name="T">Type of the object to be instantinated.</typeparam>
public interface ILazy<T>
{
    /// <summary>
    /// Grants access to the instance of an object of type <see cref="{T}"/> 
    /// with lazy instantination.
    /// </summary>
    /// <returns>Stored object of type<see cref="{T}"/>.</returns>
    T Get();
}