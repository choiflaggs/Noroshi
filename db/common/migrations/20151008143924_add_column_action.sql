
-- +goose Up
-- SQL in section 'Up' is executed when this migration is applied

ALTER TABLE `action`
  ADD COLUMN `SoundID` int unsigned NOT NULL AFTER `HitCharacterEffectID`,
  ADD COLUMN `ExecutionSoundID` int unsigned NOT NULL AFTER `SoundID`;

-- +goose Down
-- SQL section 'Down' is executed when this migration is rolled back

ALTER TABLE `action`
  DROP COLUMN `SoundID`,
  DROP COLUMN `ExecutionSoundID`;

