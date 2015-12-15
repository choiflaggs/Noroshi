
-- +goose Up
-- SQL in section 'Up' is executed when this migration is applied

DROP TABLE `guild_raid_boss`;
CREATE TABLE `guild_raid_boss` (
  `ID` int unsigned NOT NULL,
  `GuildID` int unsigned NOT NULL,
  `RaidBossID` int unsigned NOT NULL,
  `DiscoveryPlayerID` int unsigned NOT NULL,
  `LastBattlePlayerID` int unsigned NOT NULL,
  `ComboNum` int unsigned NOT NULL,
  `BattleData` text NOT NULL,
  `EntryData` text NOT NULL,
  `CreatedAt` int unsigned NOT NULL,
  `UpdatedAt` int unsigned NOT NULL,
  PRIMARY KEY (`ID`),
  UNIQUE KEY `i1` (`GuildID`,`RaidBossID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

ALTER TABLE `player_status`
  CHANGE COLUMN `GuildID` `GuildID` int unsigned NOT NULL,
  ADD COLUMN `GuildRole` tinyint unsigned NOT NULL AFTER `GuildID`,
  ADD COLUMN `GreetedNum` smallint unsigned NOT NULL AFTER `LastGreetedAt`;

ALTER TABLE `player_relation`
  CHANGE COLUMN `LastGreetedAt` `LastGreetingAt` int unsigned NOT NULL;

CREATE TABLE `player_confirmation` (
  `PlayerID` int unsigned NOT NULL,
  `ReferenceID` tinyint unsigned NOT NULL,
  `ConfirmedAt` int unsigned NOT NULL,
  PRIMARY KEY (`PlayerID`,`ReferenceID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- +goose Down
-- SQL section 'Down' is executed when this migration is rolled back

DROP TABLE `guild_raid_boss`;
CREATE TABLE `guild_raid_boss` (
  `ID` int unsigned NOT NULL,
  `GuildID` int unsigned NOT NULL,
  `RaidBossID` int unsigned NOT NULL,
  `CreatedAt` int unsigned NOT NULL,
  `UpdatedAt` int unsigned NOT NULL,
  PRIMARY KEY (`GuildID`,`RaidBossID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

ALTER TABLE `player_status`
  CHANGE COLUMN `GuildID` `GuildID` int(10) unsigned DEFAULT '0',
  DROP COLUMN `GuildRole`,
  DROP COLUMN `GreetedNum`;

ALTER TABLE `player_relation`
  CHANGE COLUMN `LastGreetingAt` `LastGreetedAt` int unsigned NOT NULL;

DROP TABLE `player_confirmation`;

