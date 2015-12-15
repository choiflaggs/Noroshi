
-- +goose Up
-- SQL in section 'Up' is executed when this migration is applied

DROP TABLE `battle`;
CREATE TABLE `cpu_battle` (
  `ID` int unsigned NOT NULL,
  `FieldID` int unsigned NOT NULL,
  `CharacterExp` int unsigned NOT NULL,
  `Gold` int unsigned NOT NULL,
  `GachaID` int unsigned NOT NULL,
  `MinDropNum` tinyint unsigned NOT NULL,
  `MaxDropNum` tinyint unsigned NOT NULL,
  `BossCpuCharacterID` int unsigned NOT NULL,
  `BossSoulDropRatio` float unsigned NOT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE `battle_wave`;
CREATE TABLE `cpu_battle_wave` (
  `ID` int unsigned NOT NULL,
  `BattleID` int unsigned NOT NULL,
  `No` tinyint unsigned NOT NULL,
  `CpuCharacterID1` int unsigned NOT NULL,
  `CpuCharacterID2` int unsigned NOT NULL,
  `CpuCharacterID3` int unsigned NOT NULL,
  `CpuCharacterID4` int unsigned NOT NULL,
  `CpuCharacterID5` int unsigned NOT NULL,
  PRIMARY KEY (`ID`),
  UNIQUE KEY `i1` (`BattleID`,`No`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE `enemy_character`;
CREATE TABLE `cpu_character` (
  `ID` int unsigned NOT NULL,
  `CharacterID` int unsigned NOT NULL,
  `Level` smallint unsigned NOT NULL,
  `PromotionLevel` tinyint unsigned NOT NULL,
  `EvolutionLevel` tinyint unsigned NOT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- +goose Down
-- SQL section 'Down' is executed when this migration is rolled back

DROP TABLE `cpu_battle`;
CREATE TABLE `battle` (
  `ID` int(10) unsigned NOT NULL,
  `FieldID` int(10) unsigned NOT NULL,
  `Exp` int(10) unsigned NOT NULL,
  `Money` int(10) unsigned NOT NULL,
  `GachaID` int(10) unsigned NOT NULL,
  `MinDropItemNum` int(10) unsigned NOT NULL,
  `MaxDropItemNum` int(10) unsigned NOT NULL,
  `BossEnemyCharacterID` int(10) unsigned NOT NULL,
  `BossStoneDropRatio` int(10) unsigned NOT NULL,
  PRIMARY KEY (`ID`),
  UNIQUE KEY `ID_UNIQUE` (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE `cpu_battle_wave`;
CREATE TABLE `battle_wave` (
  `ID` int(10) unsigned NOT NULL,
  `BattleID` int(11) NOT NULL,
  `EnemyCharacterID1` int(10) unsigned NOT NULL,
  `EnemyCharacterID2` int(10) unsigned NOT NULL,
  `EnemyCharacterID3` int(10) unsigned NOT NULL,
  `EnemyCharacterID4` int(10) unsigned NOT NULL,
  `EnemyCharacterID5` int(10) unsigned NOT NULL,
  PRIMARY KEY (`ID`),
  UNIQUE KEY `ID_UNIQUE` (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE `cpu_character`;
CREATE TABLE `enemy_character` (
  `ID` int(10) unsigned NOT NULL,
  `CharacterID` int(10) unsigned NOT NULL,
  `Level` int(10) unsigned NOT NULL,
  `PromotionLevel` int(10) unsigned NOT NULL,
  `EvolutionLevel` int(10) unsigned NOT NULL,
  PRIMARY KEY (`ID`),
  UNIQUE KEY `ID_UNIQUE` (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

