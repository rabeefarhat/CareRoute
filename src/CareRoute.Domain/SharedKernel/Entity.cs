// src/CareRoute.Domain/SharedKernel/Entity.cs
namespace CareRoute.Domain.SharedKernel;

public abstract class Entity<TId> : IEquatable<Entity<TId>>             // ①
    where TId : struct, IEquatable<TId>                                 // ②
{
    protected Entity(TId id) => Id = id;                                // ③

    public TId Id { get; }                                              // ④

    public bool Equals(Entity<TId>? other) =>                           // ⑤
        other is not null && other.GetType() == GetType() && Id.Equals(other.Id);

    public override bool Equals(object? obj) => Equals(obj as Entity<TId>);   // ⑥

    public override int GetHashCode() => HashCode.Combine(GetType(), Id);    // ⑦

    public static bool operator ==(Entity<TId>? left, Entity<TId>? right) => Equals(left, right);   // ⑧
    public static bool operator !=(Entity<TId>? left, Entity<TId>? right) => !Equals(left, right);
}