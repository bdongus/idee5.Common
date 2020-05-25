using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace idee5.Common.Data.Tests {
    public class TestEntity : IEntity, IMasterSystemReference, IAuditedEntity
    {
        private readonly ITimeProvider _timeProvider;
        private readonly ICurrentUserIdProvider _currentUserIdProvider;

        public TestEntity(ITimeProvider timeProvider, ICurrentUserIdProvider currentUserIdProvider)
        {
            _timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
            _currentUserIdProvider = currentUserIdProvider ?? throw new ArgumentNullException(nameof(currentUserIdProvider));
            DateCreated = _timeProvider.UtcNow;
            CreatedBy = _currentUserIdProvider.GetCurrentUserId();
        }

        public int Id { get; set; }
        public string MasterSystemHierarchy { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string MasterSystemId { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Label { get; set; }

        public DateTime DateCreated { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? DateModified { get; set; }
        public string ModifiedBy { get; set; }

        public override bool Equals(object obj)
        {
            var entity = obj as TestEntity;
            return entity != null &&
                   Id == entity.Id &&
                   MasterSystemHierarchy == entity.MasterSystemHierarchy &&
                   MasterSystemId == entity.MasterSystemId &&
                   Label == entity.Label &&
                   DateCreated == entity.DateCreated &&
                   CreatedBy == entity.CreatedBy &&
                   DateModified == entity.DateModified &&
                   ModifiedBy == entity.ModifiedBy;
        }

        public override int GetHashCode()
        {
            var hashCode = -1378730577;
            hashCode = hashCode * -1521134295 + Id.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(MasterSystemHierarchy);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(MasterSystemId);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Label);
            hashCode = hashCode * -1521134295 + DateCreated.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(CreatedBy);
            hashCode = hashCode * -1521134295 + EqualityComparer<DateTime?>.Default.GetHashCode(DateModified);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ModifiedBy);
            return hashCode;
        }
    }
}