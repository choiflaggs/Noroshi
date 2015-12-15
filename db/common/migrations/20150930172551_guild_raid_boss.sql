
-- +goose Up
-- SQL in section 'Up' is executed when this migration is applied

CREATE TABLE `raid_boss` (
  `ID` int unsigned NOT NULL,
  `CharacterID` int unsigned NOT NULL,
  `Level` tinyint unsigned NOT NULL,
  `CpuBattleID` int unsigned NOT NULL,
  `GuildClusterID` int unsigned NOT NULL,
  `EncounterRatio` float unsigned NOT NULL,
  PRIMARY KEY (`ID`),
  UNIQUE KEY `i1` (`CharacterID`,`Level`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE TABLE `raid_boss_reward` (
  `RaidBossID` int unsigned NOT NULL,
  `Category` tinyint unsigned NOT NULL,
  `No` tinyint unsigned NOT NULL,
  `PossessionCategory` tinyint unsigned NOT NULL,
  `PossessionID` int unsigned NOT NULL,
  `PossessionNum` int unsigned NOT NULL,
  PRIMARY KEY (`RaidBossID`,`Category`,`No`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE TABLE `sequential_id_guild` (
  `ID` int unsigned NOT NULL
) ENGINE=MyISAM;
INSERT INTO sequential_id_guild VALUES (0);

-- +goose Down
-- SQL section 'Down' is executed when this migration is rolled back

DROP TABLE `raid_boss`;
DROP TABLE `raid_boss_reward`;
DROP TABLE `sequential_id_guild`;

