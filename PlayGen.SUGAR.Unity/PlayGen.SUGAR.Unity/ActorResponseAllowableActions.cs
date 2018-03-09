using PlayGen.SUGAR.Contracts.Shared;

namespace PlayGen.SUGAR.Unity
{
	/// <summary>
	/// ActorResponse with additional information on if the current user can add and remove them.
	/// </summary>
	public class ActorResponseAllowableActions
	{
		/// <summary>
		/// ActorResponse contains the actor ID and Name.
		/// </summary>
		public ActorResponse Actor { get; set; }
		/// <summary>
		/// Can the currently signed in user add or accept a request from this actor?
		/// </summary>
		public bool CanAdd { get; set; }
		/// <summary>
		/// Can the currently signed in user remove or reject a request from this actor?
		/// </summary>
		public bool CanRemove { get; set; }

		public ActorResponseAllowableActions (ActorResponse actor, bool add, bool remove)
		{
			Actor = actor;
			CanAdd = add;
			CanRemove = remove;
		}
	}
}
