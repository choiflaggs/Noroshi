
-- +goose Up
-- SQL in section 'Up' is executed when this migration is applied

ALTER TABLE `training`
  ADD COLUMN `Type` tinyint(3) unsigned NOT NULL AFTER `TextKey`,
  ADD COLUMN `Sunday` tinyint(3) unsigned NOT NULL AFTER `ClosedAt`,
  ADD COLUMN `Monday` tinyint(3) unsigned NOT NULL AFTER `Sunday`,
  ADD COLUMN `Tuesday` tinyint(3) unsigned NOT NULL AFTER `Monday`,
  ADD COLUMN `Wednesday` tinyint(3) unsigned NOT NULL AFTER `Tuesday`,
  ADD COLUMN `Thursday` tinyint(3) unsigned NOT NULL AFTER `Wednesday`,
  ADD COLUMN `Friday` tinyint(3) unsigned NOT NULL AFTER `Thursday`,
  ADD COLUMN `Saturday` tinyint(3) unsigned NOT NULL AFTER `Friday`,
  ADD UNIQUE KEY `TextKey_UNIQUE` (`TextKey`);

ALTER TABLE `training_stage`
  ADD COLUMN `CharacterExpCoefficient` float unsigned NOT NULL AFTER `Level`,
  ADD COLUMN `GoldCoefficient` float unsigned NOT NULL AFTER `CharacterExpCoefficient`,
  CHANGE COLUMN `PlayerLevel` `NecessaryPlayerLevel` smallint unsigned NOT NULL,
  CHANGE COLUMN `BattleID` `CpuBattleID` int unsigned NOT NULL,
  DEFAULT CHARACTER SET = utf8;

DROP TABLE `training_date`;

-- +goose Down
-- SQL section 'Down' is executed when this migration is rolled back

ALTER TABLE `training`
  DROP COLUMN `Type`,
  DROP COLUMN `Sunday`,
  DROP COLUMN `Monday`,
  DROP COLUMN `Tuesday`,
  DROP COLUMN `Wednesday`,
  DROP COLUMN `Thursday`,
  DROP COLUMN `Friday`,
  DROP COLUMN `Saturday`,
  DROP KEY `TextKey_UNIQUE`;

ALTER TABLE `training_stage`
  DROP COLUMN `CharacterExpCoefficient`,
  DROP COLUMN `GoldCoefficient`,
  CHANGE COLUMN `NecessaryPlayerLevel` `PlayerLevel` smallint unsigned NOT NULL,
  CHANGE COLUMN `CpuBattleID` `BattleID` int unsigned NOT NULL,
  DEFAULT CHARACTER SET = utf8 COLLATE=utf8_unicode_ci;

CREATE TABLE `training_date` (
  `TrainingID` int(10) unsigned NOT NULL,
  `Day` tinyint(3) unsigned NOT NULL,
  PRIMARY KEY (`TrainingID`,`Day`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

