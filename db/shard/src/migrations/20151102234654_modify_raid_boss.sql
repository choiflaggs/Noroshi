
-- +goose Up
-- SQL in section 'Up' is executed when this migration is applied

ALTER TABLE `guild_raid_boss`
  DROP COLUMN `EntryData`,
  DROP KEY `i1`,
  DROP KEY `i2`,
  ADD KEY `i1` (`GuildID`,`State`,`CreatedAt`);

DROP TABLE `guild_raid_boss_log`;
CREATE TABLE `guild_raid_boss_log` (
  `ID` int unsigned NOT NULL,
  `GuildRaidBossID` int unsigned NOT NULL,
  `PlayerID` int unsigned NOT NULL,
  `Damage` int unsigned NOT NULL,
  `GuildRaidBossCreatedAt` int unsigned NOT NULL,
  `CreatedAt` int unsigned NOT NULL,
  PRIMARY KEY (`ID`,`GuildRaidBossCreatedAt`),
  KEY `i1` (`GuildRaidBossID`,`CreatedAt`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- +goose Down
-- SQL section 'Down' is executed when this migration is rolled back

ALTER TABLE `guild_raid_boss`
  ADD COLUMN `EntryData` text NOT NULL AFTER `BattleData`,
  DROP KEY `i1`,
  ADD UNIQUE KEY `i1` (`GuildID`,`RaidBossID`),
  ADD KEY `i2` (`GuildID`,`State`,`DefeatedAt`);

DROP TABLE `guild_raid_boss_log`;
CREATE TABLE `guild_raid_boss_log` (
  `GuildRaidBossID` int(10) unsigned NOT NULL,
  `PlayerID` int(10) unsigned NOT NULL,
  `Damage` int(10) unsigned NOT NULL,
  `CreatedAt` int(10) unsigned NOT NULL,
  PRIMARY KEY (`GuildRaidBossID`,`PlayerID`,`CreatedAt`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

