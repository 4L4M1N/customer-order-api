using System;

namespace Domain
{
    public interface ISoftDeletedEntity
    {
        bool IsDeleted { get; }
        DateTime? DeletedOnUtc { get; }
        void SoftDelete();
    }
}