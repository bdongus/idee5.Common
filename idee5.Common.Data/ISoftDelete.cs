using System;

namespace idee5.Common.Data {
	/// <summary>
	/// Used to standardize soft deleting entities.
	/// Soft-delete entities are not actually deleted, but
	/// marked as IsDeleted = true.
	/// </summary>
	public interface ISoftDelete {
		/// <summary>
		/// Soft delete this instance.
		/// </summary>
		/// <param name="deletedBy">User or program deleting this instance.</param>
		void Delete(string deletedBy);

		/// <summary>
		/// Undelete this soft deleted instance.
		/// </summary>
		/// <param name="undeletedBy">User or program undeleting this instance.</param>
		void Undelete(string undeletedBy);

		/// <summary>
		/// Used to mark an entity as 'Deleted'.
		/// </summary>
		bool IsDeleted { get; }

		/// <summary>
		/// Date and time this instance was marked as deleted. Usually in UTC.
		/// </summary>
		DateTime? DateDeleted { get; }

		/// <summary>
		/// By whom this instance was deleted.
		/// </summary>
		string DeletedBy { get; }

		/// <summary>
		/// Date and time this instance as undeleted. Usually in UTC.
		/// </summary>
		DateTime? DateUndeleted { get; }

		/// <summary>
		/// By whom this instance was undeleted.
		/// </summary>
		string UndeletedBy { get; }
	}
}