
-- +goose Up
-- SQL in section 'Up' is executed when this migration is applied

ALTER TABLE `trial`
  ADD COLUMN `TagFlags` text NOT NULL AFTER `TextKey`,
  ADD COLUMN `Sunday` tinyint unsigned NOT NULL AFTER `ClosedAt`,
  ADD COLUMN `Monday` tinyint unsigned NOT NULL AFTER `Sunday`,
  ADD COLUMN `Tuesday` tinyint unsigned NOT NULL AFTER `Monday`,
  ADD COLUMN `Wednesday` tinyint unsigned NOT NULL AFTER `Tuesday`,
  ADD COLUMN `Thursday` tinyint unsigned NOT NULL AFTER `Wednesday`,
  ADD COLUMN `Friday` tinyint unsigned NOT NULL AFTER `Thursday`,
  ADD COLUMN `Saturday` tinyint unsigned NOT NULL AFTER `Friday`,
  ADD UNIQUE KEY `TextKey_UNIQUE` (`TextKey`);

ALTER TABLE `trial_stage`
  DROP COLUMN `PlayerLevel`,
  CHANGE COLUMN `BattleID` `CpuBattleID` int unsigned NOT NULL;

DROP TABLE `trial_date`;

-- +goose Down
-- SQL section 'Down' is executed when this migration is rolled back

ALTER TABLE `trial`
  DROP COLUMN `TagFlags`,
  DROP COLUMN `Sunday`,
  DROP COLUMN `Monday`,
  DROP COLUMN `Tuesday`,
  DROP COLUMN `Wednesday`,
  DROP COLUMN `Thursday`,
  DROP COLUMN `Friday`,
  DROP COLUMN `Saturday`,
  DROP KEY `TextKey_UNIQUE`;

ALTER TABLE `trial_stage`
  ADD COLUMN `PlayerLevel` smallint unsigned NOT NULL AFTER `Level`,
  CHANGE COLUMN `CpuBattleID` `BattleID` int unsigned NOT NULL;

CREATE TABLE `trial_date` (
  `TrialID` int(10) unsigned NOT NULL,
  `Day` tinyint(3) unsigned NOT NULL,
  PRIMARY KEY (`TrialID`,`Day`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

