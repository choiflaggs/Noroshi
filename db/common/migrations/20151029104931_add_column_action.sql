
-- +goose Up
-- SQL in section 'Up' is executed when this migration is applied

ALTER TABLE `action`
  ADD COLUMN `ExecutorTargetable` tinyint unsigned NOT NULL AFTER `ExecutableNum`;

-- +goose Down
-- SQL section 'Down' is executed when this migration is rolled back

ALTER TABLE `action`
  DROP COLUMN `ExecutorTargetable`;

