using PlayGen.SUGAR.Contracts.Shared;

namespace PlayGen.SUGAR.Unity
{
	public class ActorResponseAllowableActions
	{
		public ActorResponse Actor { get; set; }
		public bool CanAdd { get; set; }
		public bool CanRemove { get; set; }

		public ActorResponseAllowableActions (ActorResponse actor, bool add, bool remove)
		{
			Actor = actor;
			CanAdd = add;
			CanRemove = remove;
		}
	}
}
