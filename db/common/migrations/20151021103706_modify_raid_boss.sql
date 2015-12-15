
-- +goose Up
-- SQL in section 'Up' is executed when this migration is applied

CREATE TABLE `raid_boss_group` (
  `ID` int unsigned NOT NULL,
  `TextKey` varchar(32) NOT NULL,
  `Type` tinyint unsigned NOT NULL,
  `CharacterID` int unsigned NOT NULL,
  `OpenedAt` int unsigned NOT NULL,
  `ClosedAt` int unsigned NOT NULL,
  PRIMARY KEY (`ID`),
  UNIQUE KEY `i1` (`TextKey`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE `raid_boss`;
CREATE TABLE `raid_boss` (
  `ID` int unsigned NOT NULL,
  `GroupID` int unsigned NOT NULL,
  `Level` tinyint unsigned NOT NULL,
  `CpuBattleID` int unsigned NOT NULL,
  `GuildClusterID` int unsigned NOT NULL,
  `EncounterProbability` float unsigned NOT NULL,
  `LifetimeMinute` smallint unsigned NOT NULL,
  PRIMARY KEY (`ID`),
  UNIQUE KEY `i1` (`GroupID`,`Level`),
  KEY `i2` (`GuildClusterID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- +goose Down
-- SQL section 'Down' is executed when this migration is rolled back

DROP TABLE `raid_boss_group`;

DROP TABLE `raid_boss`;
CREATE TABLE `raid_boss` (
  `ID` int(10) unsigned NOT NULL,
  `CharacterID` int(10) unsigned NOT NULL,
  `Level` tinyint(3) unsigned NOT NULL,
  `CpuBattleID` int(10) unsigned NOT NULL,
  `GuildClusterID` int(10) unsigned NOT NULL,
  `EncounterRatio` float unsigned NOT NULL,
  PRIMARY KEY (`ID`),
  UNIQUE KEY `i1` (`CharacterID`,`Level`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

