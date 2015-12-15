
-- +goose Up
-- SQL in section 'Up' is executed when this migration is applied

ALTER TABLE `player_training_stage`
  CHANGE COLUMN `Rank` `Score` int unsigned NOT NULL,
  DEFAULT CHARACTER SET = utf8;

DROP TABLE `player_training`;
CREATE TABLE `player_training` (
  `PlayerID` int unsigned NOT NULL,
  `TrainingID` int unsigned NOT NULL,
  `LastBattleNum` tinyint unsigned NOT NULL,
  `LastBattledAt` int unsigned NOT NULL,
  `ReopenedAt` int unsigned NOT NULL,
  `CreatedAt` int unsigned NOT NULL,
  `UpdatedAt` int unsigned NOT NULL,
  PRIMARY KEY (`PlayerID`,`TrainingID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- +goose Down
-- SQL section 'Down' is executed when this migration is rolled back

ALTER TABLE `player_training_stage`
  CHANGE COLUMN `Score` `Rank` tinyint unsigned NOT NULL,
  DEFAULT CHARACTER SET = utf8 COLLATE=utf8_unicode_ci;

DROP TABLE `player_training`;
CREATE TABLE `player_training` (
  `PlayerID` int(10) unsigned NOT NULL,
  `TrainingID` int(10) unsigned NOT NULL,
  `PlayCount` int(10) unsigned NOT NULL,
  `PlayMaxCount` int(10) unsigned NOT NULL,
  `CoolTime` int(10) unsigned NOT NULL,
  `ResetAt` int(10) unsigned NOT NULL,
  `CreatedAt` int(10) unsigned NOT NULL,
  `UpdatedAt` int(10) unsigned NOT NULL,
  PRIMARY KEY (`PlayerID`,`TrainingID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

