/// <summary>
/// A dependency that is created after the VContainer scope is built (#142 slice 4).
/// <see cref="PlayerInventory"/> and <see cref="TargetManager"/> live in UI/Input prefabs
/// instantiated after the scope, so a plain <c>[Inject]</c> constructor parameter would be
/// null and the registration is absent. Consumers inject the provider instead and get null
/// until the object exists, which is the same contract the static locator offered.
/// </summary>
public interface ILateBound<out T> where T : class
{
    /// <summary>The live instance, or null while it has not been created.</summary>
    T Value { get; }
}
