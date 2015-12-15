
-- +goose Up
-- SQL in section 'Up' is executed when this migration is applied

ALTER TABLE `player_status`
  CHANGE COLUMN `StaminaTime` `LastStamina` smallint unsigned NOT NULL,
  CHANGE COLUMN `StaminaOverCount` `LastStaminaUpdatedAt` int unsigned NOT NULL,
  ADD COLUMN `LastGreetingNum` tinyint unsigned NOT NULL AFTER `GuildID`,
  ADD COLUMN `LastGreetedAt` int unsigned NOT NULL AFTER `LastGreetingNum`,
  ADD COLUMN `LastBP` tinyint unsigned NOT NULL AFTER `LastGreetedAt`,
  ADD COLUMN `LastBPUpdatedAt` int unsigned NOT NULL AFTER `LastBP`,
  ADD KEY `i1` (`GuildID`);

CREATE TABLE `guild` (
  `ID` int unsigned NOT NULL,
  `Category` tinyint unsigned NOT NULL,
  `Name` text NOT NULL,
  `Introduction` text NOT NULL,
  `OwnerPlayerID` int unsigned NOT NULL,
  `MemberNum` smallint unsigned NOT NULL,
  `JoinPermission` tinyint unsigned NOT NULL,
  `ChatID` text NOT NULL,
  `CreatedAt` int unsigned NOT NULL,
  `UpdatedAt` int unsigned NOT NULL,
  PRIMARY KEY (`ID`),
  KEY `i1` (`Category`,`MemberNum`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE TABLE `player_relation` (
  `PlayerID` int unsigned NOT NULL,
  `TargetPlayerID` int unsigned NOT NULL,
  `LastGreetedAt` int unsigned NOT NULL,
  `CreatedAt` int unsigned NOT NULL,
  `UpdatedAt` int unsigned NOT NULL,
  PRIMARY KEY (`PlayerID`,`TargetPlayerID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE TABLE `player_rental_character` (
  `PlayerID` int unsigned NOT NULL,
  `No` tinyint unsigned NOT NULL,
  `PlayerCharacterID` int unsigned NOT NULL,
  `CreatedAt` int unsigned NOT NULL,
  `UpdatedAt` int unsigned NOT NULL,
  PRIMARY KEY (`PlayerID`,`No`),
  UNIQUE KEY `i1` (`PlayerID`,`PlayerCharacterID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE TABLE `guild_raid_boss` (
  `ID` int unsigned NOT NULL,
  `GuildID` int unsigned NOT NULL,
  `RaidBossID` int unsigned NOT NULL,
  `CreatedAt` int unsigned NOT NULL,
  `UpdatedAt` int unsigned NOT NULL,
  PRIMARY KEY (`GuildID`,`RaidBossID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE TABLE `guild_raid_boss_event` (
  `GuildRaidBossID` int unsigned NOT NULL,
  `PlayerID` int unsigned NOT NULL,
  `Damage` int unsigned NOT NULL,
  `CreatedAt` int unsigned NOT NULL,
  PRIMARY KEY (`GuildRaidBossID`,`CreatedAt`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- +goose Down
-- SQL section 'Down' is executed when this migration is rolled back

ALTER TABLE `player_status`
  CHANGE COLUMN `LastStamina` `StaminaTime` int(10) unsigned NOT NULL,
  CHANGE COLUMN `LastStaminaUpdatedAt` `StaminaOverCount` int(11) unsigned DEFAULT '0',
  DROP COLUMN `LastGreetingNum`,
  DROP COLUMN `LastGreetedAt`,
  DROP COLUMN `LastBP`,
  DROP COLUMN `LastBPUpdatedAt`,
  DROP KEY `i1`;

DROP TABLE `guild`;
DROP TABLE `player_relation`;
DROP TABLE `player_rental_character`;
DROP TABLE `guild_raid_boss`;
DROP TABLE `guild_raid_boss_event`;

