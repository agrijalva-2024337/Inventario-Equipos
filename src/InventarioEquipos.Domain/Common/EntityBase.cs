namespace InventarioEquipos.Domain.Common;

public abstract class EntityBase
{
    public int Id { get; protected set; }
    protected EntityBase() { }
    protected EntityBase(int id)
    {
        if (id <= 0)
            throw new ArgumentException("El Id de la entidad debe ser mayor a 0.", nameof(id));

        Id = id;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not EntityBase other) return false;
        if (ReferenceEquals(this, other)) return true;
        if (GetType() != other.GetType()) return false;
        if (Id == 0 || other.Id == 0) return false;
        return Id == other.Id;
    }

    public override int GetHashCode() => HashCode.Combine(GetType(), Id);

    public static bool operator ==(EntityBase? left, EntityBase? right)
        => left is null ? right is null : left.Equals(right);

    public static bool operator !=(EntityBase? left, EntityBase? right) => !(left == right);
}
