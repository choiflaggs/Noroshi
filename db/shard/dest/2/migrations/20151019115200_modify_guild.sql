
-- +goose Up
-- SQL in section 'Up' is executed when this migration is applied

ALTER TABLE `guild`
  DROP COLUMN `JoinPermission`,
  CHANGE COLUMN `MemberNum` `MemberNum` int unsigned NOT NULL,
  ADD COLUMN `CountryID` int unsigned NOT NULL AFTER `Category`,
  ADD COLUMN `NecessaryPlayerLevel` smallint unsigned NOT NULL AFTER `CountryID`;

ALTER TABLE `guild_raid_boss`
  ADD COLUMN `State` tinyint unsigned NOT NULL AFTER `RaidBossID`;

DROP TABLE `guild_raid_boss_event`;
CREATE TABLE `guild_raid_boss_log` (
  `GuildRaidBossID` int unsigned NOT NULL,
  `PlayerID` int unsigned NOT NULL,
  `Damage` int unsigned NOT NULL,
  `CreatedAt` int unsigned NOT NULL,
  PRIMARY KEY (`GuildRaidBossID`,`PlayerID`,`CreatedAt`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE TABLE `player_guild_raid_boss` (
  `PlayerID` int unsigned NOT NULL,
  `GuildRaidBossID` int unsigned NOT NULL,
  `State` tinyint unsigned NOT NULL,
  `Damage` int unsigned NOT NULL,
  `GuildRaidBossCreatedAt` int unsigned NOT NULL,
  `UpdatedAt` int unsigned NOT NULL,
  PRIMARY KEY (`PlayerID`,`GuildRaidBossID`,`GuildRaidBossCreatedAt`),
  KEY `i1` (`GuildRaidBossID`,`Damage`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE TABLE `guild_activity_daily_log` (
  `GuildID` int unsigned NOT NULL,
  `BPConsuming` int unsigned NOT NULL,
  `CreatedOn` int unsigned NOT NULL,
  PRIMARY KEY (`GuildID`,`CreatedOn`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- +goose Down
-- SQL section 'Down' is executed when this migration is rolled back

ALTER TABLE `guild`
  ADD COLUMN `JoinPermission` tinyint unsigned NOT NULL AFTER `RequestNum`,
  CHANGE COLUMN `MemberNum` `MemberNum` smallint unsigned NOT NULL,
  DROP COLUMN `CountryID`,
  DROP COLUMN `NecessaryPlayerLevel`;

ALTER TABLE `guild_raid_boss`
  DROP COLUMN `State`;

DROP TABLE `guild_raid_boss_log`;
CREATE TABLE `guild_raid_boss_event` (
  `GuildRaidBossID` int(10) unsigned NOT NULL,
  `PlayerID` int(10) unsigned NOT NULL,
  `Damage` int(10) unsigned NOT NULL,
  `CreatedAt` int(10) unsigned NOT NULL,
  PRIMARY KEY (`GuildRaidBossID`,`CreatedAt`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE `player_guild_raid_boss`;

DROP TABLE `guild_activity_daily_log`;

