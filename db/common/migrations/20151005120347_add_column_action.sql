
-- +goose Up
-- SQL in section 'Up' is executed when this migration is applied

ALTER TABLE `action`
  ADD COLUMN `TargetSortType` tinyint unsigned NOT NULL AFTER `Description`,
  ADD COLUMN `MaxTargetNum` tinyint unsigned NOT NULL AFTER `TargetSortType`;

-- +goose Down
-- SQL section 'Down' is executed when this migration is rolled back

ALTER TABLE `action`
  DROP COLUMN `TargetSortType`,
  DROP COLUMN `MaxTargetNum`;

