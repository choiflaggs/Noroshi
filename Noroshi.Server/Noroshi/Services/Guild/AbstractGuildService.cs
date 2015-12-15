using System;
using System.Collections.Generic;
using System.Linq;
using Noroshi.Core.WebApi.Response.Guild;
using Noroshi.Core.Game.Guild;
using Noroshi.Server.Entity.Player;
using Noroshi.Server.Entity.Guild;
using Noroshi.Server.Daos.Rdb;

namespace Noroshi.Server.Services.Guild
{
    public abstract class AbstractGuildService
    {
        // 自プレイヤーがギルドに所属しているかどうかチェック。
        protected static T _validateInGuild<T>(uint playerId, ReadType readType, Func<PlayerStatusEntity, GuildEntity, GuildError, T> func)
        {
            return _build(playerId, readType, (playerStatus, guild, request, requestGuild) =>
            {
                if (guild == null) return func.Invoke(playerStatus, guild, new GuildError { GuildNotFound = true });
                return func.Invoke(playerStatus, guild, null);
            });
        }
        // 同じギルドに所属する他プレイヤーチェック。
        protected static T _validateWithOtherPlayerInSameGuild<T>(uint playerId, uint targetPlayerId, ReadType readType, Func<PlayerStatusEntity, PlayerStatusEntity, GuildEntity, GuildError, T> func)
        {
            if (playerId == targetPlayerId)
            {
                throw new InvalidOperationException(string.Join("\t", "Is Same Player", playerId, targetPlayerId));
            }
            return _buildWithTargetPlayer(playerId, targetPlayerId, readType, (playerStatus, guild, request, requestGuild, targetPlayerStatus, targetGuild, targetRequest, targetRequestGuild) =>
            {
                if (guild == null)
                {
                    return func(playerStatus, targetPlayerStatus, null, new GuildError { GuildNotFound = true });
                }
                if (targetGuild == null)
                {
                    return func(playerStatus, targetPlayerStatus, null, new GuildError { TargetGuildNotFound = true });
                }
                if (guild.ID != targetGuild.ID)
                {
                    return func(playerStatus, targetPlayerStatus, null, new GuildError { IsNotSameGuild = true });
                }
                return func.Invoke(playerStatus, targetPlayerStatus, guild, null);
            });
        }
        // 他ギルドチェック。
        protected static T _validateWithOtherGuild<T>(uint playerId, uint targetGuildId, ReadType readType, Func<PlayerStatusEntity, GuildEntity, GuildRequestEntity, GuildEntity, GuildEntity, GuildError, T> func)
        {
            return _buildWithTargetGuild(playerId, targetGuildId, readType, (playerStatus, currentGuild, guildRequest, requestGuild, targetGuild) =>
            {
                if (currentGuild != null && targetGuild != null && currentGuild.ID == targetGuild.ID)
                {
                    throw new InvalidOperationException(string.Join("\t", "Target Guild Is Own Guild", targetGuildId));
                }
                if (targetGuild == null)
                {
                    return func.Invoke(playerStatus, currentGuild, guildRequest, requestGuild, targetGuild, new GuildError { TargetGuildNotFound = true });
                }
                return func.Invoke(playerStatus, currentGuild, guildRequest, requestGuild, targetGuild, null);
            });
        }
        protected static PlayerStatusEntity _buildOwnPlayerStatus(uint playerId, ReadType readType)
        {
            var playerStatus = PlayerStatusEntity.ReadAndBuild(playerId, readType);
            if (playerStatus == null)
            {
                throw new SystemException(string.Join("\t", "Player Status Not Found", playerId));
            }
            return playerStatus;
        }
        protected static T _build<T>(uint playerId, ReadType readType, Func<PlayerStatusEntity, GuildEntity, GuildRequestEntity, GuildEntity, T> func)
        {
            return _buildWithTargetGuild(playerId, null, readType, (playerStatus, currentGuild, guildRequest, requestGuild, targetGuild) =>
            {
                return func.Invoke(playerStatus, currentGuild, guildRequest, requestGuild);
            });
        }
        protected static T _buildWithTargetGuild<T>(uint playerId, uint? targetGuildId, ReadType readType, Func<PlayerStatusEntity, GuildEntity, GuildRequestEntity, GuildEntity, GuildEntity, T> func)
        {
            // PlayerStatus をビルド。
            var playerStatus = _buildOwnPlayerStatus(playerId, readType);
            // リクエストをビルド。
            var guildRequest = playerStatus.GuildState == PlayerGuildState.Request ? GuildRequestEntity.ReadAndBuild(playerId, readType) : null;

            var guildIds = new List<uint>();
            if (playerStatus.GuildID.HasValue) guildIds.Add(playerStatus.GuildID.Value);
            if (guildRequest != null) guildIds.Add(guildRequest.GuildID);
            if (targetGuildId.HasValue) guildIds.Add(targetGuildId.Value);
            guildIds = guildIds.Distinct().ToList();

            // ギルドをビルド。
            var guildMap = guildIds.Count() > 0 ? GuildEntity.ReadAndBuildMulti(guildIds, readType).ToDictionary(g => g.ID) : new Dictionary<uint, GuildEntity>();
            GuildEntity currentGuild = null;
            if (playerStatus.GuildID.HasValue)
            {
                if (!guildMap.ContainsKey(playerStatus.GuildID.Value))
                {
                    throw new SystemException(string.Join("\t", "Current Guild Not Found", playerStatus.GuildID.Value));
                }
                currentGuild = guildMap[playerStatus.GuildID.Value];
            }
            GuildEntity requestGuild = null;
            if (guildRequest != null)
            {
                if (!guildMap.ContainsKey(guildRequest.GuildID))
                {
                    throw new InvalidOperationException(string.Join("\t", "Request Guild Not Found", guildRequest.GuildID));
                }
                requestGuild = guildMap[guildRequest.GuildID];
            }
            GuildEntity targetGuild = null;
            if (targetGuildId.HasValue)
            {
                if (!guildMap.ContainsKey(targetGuildId.Value))
                {
                    throw new InvalidOperationException(string.Join("\t", "Target Guild Not Found", targetGuildId.Value));
                }
                targetGuild = guildMap[targetGuildId.Value];
            }

            return func.Invoke(playerStatus, currentGuild, guildRequest, requestGuild, targetGuild);
        }

        protected static T _buildWithTargetPlayer<T>(uint playerId, uint targetPlayerId, ReadType readType, Func<PlayerStatusEntity, GuildEntity, GuildRequestEntity, GuildEntity, PlayerStatusEntity, GuildEntity, GuildRequestEntity, GuildEntity, T> func)
        {
            // 対象プレイヤーと自プレイヤーをビルド。
            var playerStatusMap = PlayerStatusEntity.ReadAndBuildMulti(new uint[] { playerId, targetPlayerId }, readType).ToDictionary(ps => ps.PlayerID);
            if (!playerStatusMap.ContainsKey(playerId))
            {
                throw new SystemException(string.Join("\t", "Player Status Not Found", playerId));
            }
            if (!playerStatusMap.ContainsKey(targetPlayerId))
            {
                throw new InvalidOperationException(string.Join("\t", "Target Player Status Not Found", targetPlayerId));
            }
            var playerStatus = playerStatusMap[playerId];
            var targetPlayerStatus = playerStatusMap[targetPlayerId];

            // リクエストをビルド。
            var requestPlayerIds = new List<uint>();
            if (playerStatus.GuildState == PlayerGuildState.Request) requestPlayerIds.Add(playerStatus.PlayerID);
            if (targetPlayerStatus.GuildState == PlayerGuildState.Request) requestPlayerIds.Add(targetPlayerStatus.PlayerID);
            var guildRequestMap = requestPlayerIds.Count() > 0 ? GuildRequestEntity.ReadAndBuildMulti(requestPlayerIds, readType).ToDictionary(req => req.PlayerID) : new Dictionary<uint, GuildRequestEntity>();

            var guildRequest = playerStatus.GuildState == PlayerGuildState.Request ? guildRequestMap[playerStatus.PlayerID] : null;
            var targetGuildRequest = targetPlayerStatus.GuildState == PlayerGuildState.Request ? guildRequestMap[targetPlayerStatus.PlayerID] : null;

            var guildIds = new List<uint>();
            if (playerStatus.GuildID.HasValue) guildIds.Add(playerStatus.GuildID.Value);
            if (targetPlayerStatus.GuildID.HasValue) guildIds.Add(targetPlayerStatus.GuildID.Value);
            if (guildRequest != null) guildIds.Add(guildRequest.GuildID);
            if (targetGuildRequest != null) guildIds.Add(targetGuildRequest.GuildID);
            guildIds = guildIds.Distinct().ToList();

            // ギルドをビルド。
            var guildMap = guildIds.Count() > 0 ? GuildEntity.ReadAndBuildMulti(guildIds, readType).ToDictionary(g => g.ID) : new Dictionary<uint, GuildEntity>();
            GuildEntity currentGuild = null;
            if (playerStatus.GuildID.HasValue)
            {
                if (!guildMap.ContainsKey(playerStatus.GuildID.Value))
                {
                    throw new SystemException(string.Join("\t", "Current Guild Not Found", playerStatus.GuildID.Value));
                }
                currentGuild = guildMap[playerStatus.GuildID.Value];
            }
            GuildEntity targetGuild = null;
            if (targetPlayerStatus.GuildID.HasValue)
            {
                if (!guildMap.ContainsKey(targetPlayerStatus.GuildID.Value))
                {
                    throw new SystemException(string.Join("\t", "Target Guild Not Found", targetPlayerStatus.GuildID.Value));
                }
                targetGuild = guildMap[targetPlayerStatus.GuildID.Value];
            }
            GuildEntity requestGuild = null;
            if (guildRequest != null)
            {
                if (!guildMap.ContainsKey(guildRequest.GuildID))
                {
                    throw new InvalidOperationException(string.Join("\t", "Request Guild Not Found", guildRequest.GuildID));
                }
                requestGuild = guildMap[guildRequest.GuildID];
            }
            GuildEntity targetRequestGuild = null;
            if (targetGuildRequest != null)
            {
                if (!guildMap.ContainsKey(targetGuildRequest.GuildID))
                {
                    throw new InvalidOperationException(string.Join("\t", "Target Request Guild Not Found", targetGuildRequest.GuildID));
                }
                targetRequestGuild = guildMap[targetGuildRequest.GuildID];
            }

            return func.Invoke(playerStatus, currentGuild, guildRequest, requestGuild, targetPlayerStatus, targetGuild, targetGuildRequest, targetRequestGuild);
        }
    }
}
