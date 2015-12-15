
-- +goose Up
-- SQL in section 'Up' is executed when this migration is applied

ALTER TABLE `player_status`
  ADD KEY `i2` (`Exp`);

ALTER TABLE `player_expedition`
  CHANGE COLUMN `Level` `ClearLevel` tinyint unsigned NOT NULL,
  CHANGE COLUMN `ResetNum` `LastResetNum` tinyint unsigned NOT NULL,
  CHANGE COLUMN `ResetedAt` `LastResetedAt` int unsigned NOT NULL;

ALTER TABLE `player_expedition_session`
  ADD COLUMN `StartedAt` int unsigned NOT NULL AFTER `PlayerCharacterData`,
  CHANGE COLUMN `ClearStep` `ClearStep` tinyint unsigned NOT NULL,
  CHANGE COLUMN `CanAcquirePrize` `State` tinyint unsigned NOT NULL,
  CHANGE COLUMN `PreprocessData` `StageData` text NOT NULL;

-- +goose Down
-- SQL section 'Down' is executed when this migration is rolled back

ALTER TABLE `player_status`
  DROP KEY `i2`;

ALTER TABLE `player_expedition`
  CHANGE COLUMN `ClearLevel` `Level` tinyint unsigned NOT NULL,
  CHANGE COLUMN `LastResetNum` `ResetNum` tinyint unsigned NOT NULL,
  CHANGE COLUMN `LastResetedAt` `ResetedAt` int unsigned NOT NULL;

ALTER TABLE `player_expedition_session`
  DROP COLUMN `StartedAt`,
  CHANGE COLUMN `ClearStep` `ClearStep` tinyint NOT NULL,
  CHANGE COLUMN `State` `CanAcquirePrize` tinyint unsigned NOT NULL,
  CHANGE COLUMN `StageData` `PreprocessData` text NOT NULL;

