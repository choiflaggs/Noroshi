
-- +goose Up
-- SQL in section 'Up' is executed when this migration is applied

ALTER TABLE `raid_boss_reward`
  ADD COLUMN `Probability` float unsigned NOT NULL AFTER `No`;

-- +goose Down
-- SQL section 'Down' is executed when this migration is rolled back

ALTER TABLE `raid_boss_reward`
  DROP COLUMN `Probability`;

