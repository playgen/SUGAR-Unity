using System;
using System.Collections.Generic;
using PlayGen.SUGAR.Contracts;
using UnityEngine;

namespace PlayGen.SUGAR.Unity
{
	/// <summary>
	/// Unity Client for getting and updating groups and users and for creating and deleting groups.
	/// </summary>
	public class ActorUnityClient
	{
		/// <summary>
		/// Get a list of all groups that have been created.
		/// </summary>
		/// <param name="onComplete">Callback with a list of gathered GroupResponse results.</param>
		public void GetAllGroups(Action<IEnumerable<GroupResponse>> onComplete)
		{
			if (SUGARManager.UserSignedIn)
			{
				SUGARManager.client.Group.GetAsync(
				onComplete,
				exception =>
				{
					Debug.LogError(exception);
					onComplete(null);
				});
			}
			else
			{
				onComplete(null);
			}
		}

		/// <summary>
		/// Get a list of all groups that the current user has permissions over.
		/// </summary>
		/// <param name="onComplete">Callback with a list of gathered GroupResponse results.</param>
		public void GetGroupsControlledByCurrentUser(Action<IEnumerable<GroupResponse>> onComplete)
		{
			if (SUGARManager.UserSignedIn)
			{
				SUGARManager.client.Group.GetControlledAsync(
				onComplete,
				exception =>
				{
					Debug.LogError(exception);
					onComplete(null);
				});
			}
			else
			{
				onComplete(null);
			}
		}

		/// <summary>
		/// Get a list of all groups whose name contains the string provided.
		/// </summary>
		/// <param name="name">String to search by.</param>
		/// <param name="onComplete">Callback with a list of gathered GroupResponse results.</param>
		public void SearchGroupsByName(string name, Action<IEnumerable<GroupResponse>> onComplete)
		{
			if (SUGARManager.UserSignedIn)
			{
				SUGARManager.client.Group.GetAsync(name,
				onComplete,
				exception =>
				{
					Debug.LogError(exception);
					onComplete(null);
				});
			}
			else
			{
				onComplete(null);
			}
		}

		/// <summary>
		/// Get the group whose id matches the id provided.
		/// </summary>
		/// <param name="id">Id used to get the group.</param>
		/// <param name="onComplete">Callback with a GroupResponse result.</param>
		public void GetGroupById(int id, Action<GroupResponse> onComplete)
		{
			if (SUGARManager.UserSignedIn)
			{
				SUGARManager.client.Group.GetAsync(id,
				onComplete,
				exception =>
				{
					Debug.LogError(exception);
					onComplete(null);
				});
			}
			else
			{
				onComplete(null);
			}
		}

		/// <summary>
		/// Create a group with the provided name.
		/// </summary>
		/// <param name="name">Name the newly created group should have.</param>
		/// <param name="onComplete">Callback with a GroupResponse result for the newly created group.</param>
		public void CreateGroup(string name, Action<GroupResponse> onComplete)
		{
			if (SUGARManager.UserSignedIn)
			{
				var groupRequest = new GroupRequest
				{
					Name = name
				};
				SUGARManager.client.Group.CreateAsync(groupRequest,
				onComplete,
				exception =>
				{
					Debug.LogError(exception);
					onComplete(null);
				});
			}
			else
			{
				onComplete(null);
			}
		}

		/// <summary>
		/// Create a group with the provided name and description.
		/// </summary>
		/// <param name="name">Name the newly created group should have.</param>
		/// <param name="description">Description the newly created group should have.</param>
		/// <param name="onComplete">Callback with a GroupResponse result for the newly created group.</param>
		public void CreateGroup(string name, string description, Action<GroupResponse> onComplete)
		{
			if (SUGARManager.UserSignedIn)
			{
				var groupRequest = new GroupRequest
				{
					Name = name,
					Description = description
				};
				SUGARManager.client.Group.CreateAsync(groupRequest,
				onComplete,
				exception =>
				{
					Debug.LogError(exception);
					onComplete(null);
				});
			}
			else
			{
				onComplete(null);
			}
		}

		/// <summary>
		/// Update the group with the provided id to now have the provided name.
		/// </summary>
		/// <param name="id">The Id of the group to update.</param>
		/// <param name="name">Name the group should now have.</param>
		/// <param name="onComplete">Callback with a GroupResponse result for the newly updated group.</param>
		public void UpdateGroupName(int id, string name, Action<GroupResponse> onComplete)
		{
			if (SUGARManager.UserSignedIn)
			{
				GetGroupById(id,
				groupLoaded =>
				{
					if (groupLoaded != null)
					{
						var groupRequest = new GroupRequest
						{
							Name = name,
							Description = groupLoaded.Description
						};
						SUGARManager.client.Group.UpdateAsync(id, groupRequest,
						() => GetGroupById(id, onComplete),
						exception =>
						{
							Debug.LogError(exception);
							onComplete(null);
						});
					}
				});
			}
			else
			{
				onComplete(null);
			}
		}

		/// <summary>
		/// Update the group with the provided id to now have the provided description.
		/// </summary>
		/// <param name="id">The Id of the group to update.</param>
		/// <param name="description">Description the group should now have.</param>
		/// <param name="onComplete">Callback with a GroupResponse result for the newly updated group.</param>
		public void UpdateGroupDescription(int id, string description, Action<GroupResponse> onComplete)
		{
			if (SUGARManager.UserSignedIn)
			{
				GetGroupById(id,
				groupLoaded =>
				{
					if (groupLoaded != null)
					{
						var groupRequest = new GroupRequest
						{
							Name = groupLoaded.Name,
							Description = description
						};
						SUGARManager.client.Group.UpdateAsync(id, groupRequest,
						() => GetGroupById(id, onComplete),
						exception =>
						{
							Debug.LogError(exception);
							onComplete(null);
						});
					}
				});
			}
			else
			{
				onComplete(null);
			}
		}

		/// <summary>
		/// Update the group with the provided id to now have the provided name and description.
		/// </summary>
		/// <param name="id">The Id of the group to update.</param>
		/// <param name="name">Name the group should now have.</param>
		/// <param name="description">Description the group should now have.</param>
		/// <param name="onComplete">Callback with a GroupResponse result for the newly updated group.</param>
		public void UpdateGroup(int id, string name, string description, Action<GroupResponse> onComplete)
		{
			if (SUGARManager.UserSignedIn)
			{
				var groupRequest = new GroupRequest
				{
					Name = name,
					Description = description
				};
				SUGARManager.client.Group.UpdateAsync(id, groupRequest,
				() => GetGroupById(id, onComplete),
				exception =>
				{
					Debug.LogError(exception);
					onComplete(null);
				});
			}
			else
			{
				onComplete(null);
			}
		}

		/// <summary>
		/// Delete the group with the provided Id.
		/// </summary>
		/// <param name="id">The Id of the group to delete.</param>
		/// <param name="onComplete">Callback with a bool for whether the deletion was successful.</param>
		public void DeleteGroup(int id, Action<bool> onComplete)
		{
			if (SUGARManager.UserSignedIn)
			{
				SUGARManager.client.Group.DeleteAsync(id,
				() => onComplete(true),
				exception =>
				{
					Debug.LogError(exception);
					onComplete(false);
				});
			}
			else
			{
				onComplete(false);
			}
		}

		/// <summary>
		/// Get a list of all users whose name contains the string provided.
		/// </summary>
		/// <param name="name">String to search by.</param>
		/// <param name="onComplete">Callback with a list of gathered UserResponse results.</param>
		public void SearchUsersByName(string name, Action<IEnumerable<UserResponse>> onComplete)
		{
			if (SUGARManager.UserSignedIn)
			{
				SUGARManager.client.User.GetAsync(name,
				onComplete,
				exception =>
				{
					Debug.LogError(exception);
					onComplete(null);
				});
			}
			else
			{
				onComplete(null);
			}
		}

		/// <summary>
		/// Get the user whose id matches the id provided.
		/// </summary>
		/// <param name="id">Id used to get the user.</param>
		/// <param name="onComplete">Callback with a UserResponse result.</param>
		public void GetUserById(int id, Action<UserResponse> onComplete)
		{
			if (SUGARManager.UserSignedIn)
			{
				SUGARManager.client.User.GetAsync(id,
				onComplete,
				exception =>
				{
					Debug.LogError(exception);
					onComplete(null);
				});
			}
			else
			{
				onComplete(null);
			}
		}

		/// <summary>
		/// Update the user with the provided id to now have the provided name.
		/// </summary>
		/// <param name="id">The Id of the user to update.</param>
		/// <param name="name">Name the user should now have.</param>
		/// <param name="onComplete">Callback with a UserResponse result for the newly updated user.</param>
		public void UpdateUserName(int id, string name, Action<UserResponse> onComplete)
		{
			if (SUGARManager.UserSignedIn)
			{
				GetUserById(id,
				userLoaded =>
				{
					if (userLoaded != null)
					{
						var userRequest = new UserRequest
						{
							Name = name,
							Description = userLoaded.Description
						};
						SUGARManager.client.User.UpdateAsync(id, userRequest,
						() => GetUserById(id, onComplete),
						exception =>
						{
							Debug.LogError(exception);
							onComplete(null);
						});
					}
				});
			}
			else
			{
				onComplete(null);
			}
		}

		/// <summary>
		/// Update the user with the provided id to now have the provided description.
		/// </summary>
		/// <param name="id">The Id of the user to update.</param>
		/// <param name="description">Description the user should now have.</param>
		/// <param name="onComplete">Callback with a UserResponse result for the newly updated user.</param>
		public void UpdateUserDescription(int id, string description, Action<UserResponse> onComplete)
		{
			if (SUGARManager.UserSignedIn)
			{
				GetUserById(id,
				userLoaded =>
				{
					if (userLoaded != null)
					{
						var userRequest = new UserRequest
						{
							Name = userLoaded.Name,
							Description = description
						};
						SUGARManager.client.User.UpdateAsync(id, userRequest,
						() => GetUserById(id, onComplete),
						exception =>
						{
							Debug.LogError(exception);
							onComplete(null);
						});
					}
				});
			}
			else
			{
				onComplete(null);
			}
		}

		/// <summary>
		/// Update the user with the provided id to now have the provided name and description.
		/// </summary>
		/// <param name="id">The Id of the user to update.</param>
		/// <param name="name">Name the user should now have.</param>
		/// <param name="description">Description the user should now have.</param>
		/// <param name="onComplete">Callback with a UserResponse result for the newly updated user.</param>
		public void UpdateUser(int id, string name, string description, Action<UserResponse> onComplete)
		{
			if (SUGARManager.UserSignedIn)
			{
				var userRequest = new UserRequest
				{
					Name = name,
					Description = description
				};
				SUGARManager.client.User.UpdateAsync(id, userRequest,
				() => GetUserById(id, onComplete),
				exception =>
				{
					Debug.LogError(exception);
					onComplete(null);
				});
			}
			else
			{
				onComplete(null);
			}
		}
	}
}