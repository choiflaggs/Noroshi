
-- +goose Up
-- SQL in section 'Up' is executed when this migration is applied

ALTER TABLE `daily_quest`
 ADD `OpenHour`  tinyint(3) unsigned NOT NULL AFTER `Threshold`,
 ADD `CloseHour` tinyint(3) unsigned NOT NULL AFTER `OpenHour`;

-- +goose Down
-- SQL section 'Down' is executed when this migration is rolled back

ALTER TABLE `daily_quest`
 DROP COLUMN `OpenHour`,
 DROP COLUMN `CloseHour`;
