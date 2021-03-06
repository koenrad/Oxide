﻿using System;

using Oxide.Core;
using Oxide.Core.Libraries;
using Oxide.Core.Libraries.Covalence;

namespace Oxide.Game.Hurtworld.Libraries.Covalence
{
    /// <summary>
    /// Represents a player, either connected or not
    /// </summary>
    class HurtworldPlayer : IPlayer, IEquatable<IPlayer>
    {
        private static Permission libPerms;
        private readonly ulong steamid;

        /// <summary>
        /// Gets the last-known nickname for this player
        /// </summary>
        public string Nickname { get; private set; }

        /// <summary>
        /// Gets a unique ID for this player (unique within the current game)
        /// </summary>
        public string UniqueID { get; private set; }

        /// <summary>
        /// Gets the live player if this player is connected
        /// </summary>
        public ILivePlayer ConnectedPlayer => HurtworldCovalenceProvider.Instance.PlayerManager.GetOnlinePlayer(UniqueID);

        internal HurtworldPlayer(ulong steamID, string nickname)
        {
            // Get perms library
            if (libPerms == null) libPerms = Interface.Oxide.GetLibrary<Permission>();

            // Store user details
            Nickname = nickname;
            steamid = steamID;
            UniqueID = steamID.ToString();
        }

        #region Permissions

        /// <summary>
        /// Gets if this player has the specified permission
        /// </summary>
        /// <param name="perm"></param>
        /// <returns></returns>
        public bool HasPermission(string perm)
        {
            return libPerms.UserHasPermission(UniqueID, perm);
        }

        /// <summary>
        /// Grants the specified permission on this user
        /// </summary>
        /// <param name="perm"></param>
        public void GrantPermission(string perm)
        {
            libPerms.GrantUserPermission(UniqueID, perm, null);
        }

        /// <summary>
        /// Strips the specified permission from this user
        /// </summary>
        /// <param name="perm"></param>
        public void RevokePermission(string perm)
        {
            libPerms.RevokeUserPermission(UniqueID, perm);
        }

        /// <summary>
        /// Gets if this player belongs to the specified usergroup
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public bool BelongsToGroup(string groupName)
        {
            return libPerms.UserHasGroup(UniqueID, groupName);
        }

        /// <summary>
        /// Adds this player to the specified usergroup
        /// </summary>
        /// <param name="groupName"></param>
        public void AddToGroup(string groupName)
        {
            libPerms.AddUserGroup(UniqueID, groupName);
        }

        /// <summary>
        /// Removes this player from the specified usergroup
        /// </summary>
        /// <param name="groupName"></param>
        public void RemoveFromGroup(string groupName)
        {
            libPerms.RemoveUserGroup(UniqueID, groupName);
        }

        #endregion

        #region Administration

        public void Ban(string reason, TimeSpan duration)
        {
            // TODO
        }

        public void Unban()
        {
            // TODO
        }

        public bool IsBanned => false; // TODO: Implement once supported

        public TimeSpan BanTimeRemaining => null; // Implement once supported

        #endregion

        #region Operator Overloads

        public bool Equals(IPlayer other)
        {
            return UniqueID == other.UniqueID;
        }

        #endregion
    }
}
