
-- +goose Up
-- SQL in section 'Up' is executed when this migration is applied

ALTER TABLE `raid_boss_group`
  ADD COLUMN `Sunday` tinyint unsigned NOT NULL AFTER `ClosedAt`,
  ADD COLUMN `Monday` tinyint unsigned NOT NULL AFTER `Sunday`,
  ADD COLUMN `Tuesday` tinyint unsigned NOT NULL AFTER `Monday`,
  ADD COLUMN `Wednesday` tinyint unsigned NOT NULL AFTER `Tuesday`,
  ADD COLUMN `Thursday` tinyint unsigned NOT NULL AFTER `Wednesday`,
  ADD COLUMN `Friday` tinyint unsigned NOT NULL AFTER `Thursday`,
  ADD COLUMN `Saturday` tinyint unsigned NOT NULL AFTER `Friday`;

-- +goose Down
-- SQL section 'Down' is executed when this migration is rolled back

ALTER TABLE `raid_boss_group`
  DROP COLUMN `Sunday`,
  DROP COLUMN `Monday`,
  DROP COLUMN `Tuesday`,
  DROP COLUMN `Wednesday`,
  DROP COLUMN `Thursday`,
  DROP COLUMN `Friday`,
  DROP COLUMN `Saturday`;

