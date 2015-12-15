
-- +goose Up
-- SQL in section 'Up' is executed when this migration is applied

ALTER TABLE `daily_quest`
  ADD UNIQUE KEY `i1` (`TriggerID`,`Threshold`);

-- +goose Down
-- SQL section 'Down' is executed when this migration is rolled back

ALTER TABLE `daily_quest`
  DROP KEY `i1`;

