using System;

namespace Domain
{
    public abstract class BaseEntity
    {
        public Guid Id { get; private set; }
        protected BaseEntity()
        {
            Id = Guid.NewGuid();
        }
    }
}