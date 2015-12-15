
-- +goose Up
-- SQL in section 'Up' is executed when this migration is applied

ALTER TABLE `player_status`
  ADD COLUMN `TutorialStep` smallint unsigned NOT NULL AFTER `AvaterCharacterID`,
  CHANGE COLUMN `ID` `PlayerID` int unsigned NOT NULL,
  DROP INDEX `PlayerID_UNIQUE`;

-- +goose Down
-- SQL section 'Down' is executed when this migration is rolled back

ALTER TABLE `player_status`
  DROP COLUMN `TutorialStep`,
  CHANGE COLUMN `PlayerID` `ID` int unsigned NOT NULL,
  ADD UNIQUE KEY `PlayerID_UNIQUE` (`ID`);
