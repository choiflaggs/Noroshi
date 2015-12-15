
-- +goose Up
-- SQL in section 'Up' is executed when this migration is applied

ALTER TABLE `stage`
  ADD COLUMN `IsFixedParty` tinyint unsigned NOT NULL AFTER `BattleID`,
  ADD COLUMN `FixedCharacterID1` int unsigned NOT NULL AFTER `IsFixedParty`,
  ADD COLUMN `FixedCharacterID2` int unsigned NOT NULL AFTER `FixedCharacterID1`,
  ADD COLUMN `FixedCharacterID3` int unsigned NOT NULL AFTER `FixedCharacterID2`,
  ADD COLUMN `FixedCharacterID4` int unsigned NOT NULL AFTER `FixedCharacterID3`,
  ADD COLUMN `FixedCharacterID5` int unsigned NOT NULL AFTER `FixedCharacterID4`,
  ADD COLUMN `CpuCharacterID1` int unsigned NOT NULL AFTER `FixedCharacterID5`,
  ADD COLUMN `CpuCharacterID2` int unsigned NOT NULL AFTER `CpuCharacterID1`,
  ADD COLUMN `CpuCharacterID3` int unsigned NOT NULL AFTER `CpuCharacterID2`,
  ADD COLUMN `CpuCharacterID4` int unsigned NOT NULL AFTER `CpuCharacterID3`,
  ADD COLUMN `CpuCharacterID5` int unsigned NOT NULL AFTER `CpuCharacterID4`;

-- +goose Down
-- SQL section 'Down' is executed when this migration is rolled back

ALTER TABLE `stage`
  DROP COLUMN `IsFixedParty`,
  DROP COLUMN `FixedCharacterID1`,
  DROP COLUMN `FixedCharacterID2`,
  DROP COLUMN `FixedCharacterID3`,
  DROP COLUMN `FixedCharacterID4`,
  DROP COLUMN `FixedCharacterID5`,
  DROP COLUMN `CpuCharacterID1`,
  DROP COLUMN `CpuCharacterID2`,
  DROP COLUMN `CpuCharacterID3`,
  DROP COLUMN `CpuCharacterID4`,
  DROP COLUMN `CpuCharacterID5`;

