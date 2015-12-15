
-- +goose Up
-- SQL in section 'Up' is executed when this migration is applied

ALTER TABLE `attribute`
 ADD COLUMN `Arg11` int NOT NULL AFTER `Arg10`,
 ADD COLUMN `Arg12` int NOT NULL AFTER `Arg11`,
 ADD COLUMN `Arg13` int NOT NULL AFTER `Arg12`,
 ADD COLUMN `Arg14` int NOT NULL AFTER `Arg13`,
 ADD COLUMN `Arg15` int NOT NULL AFTER `Arg14`;

-- +goose Down
-- SQL section 'Down' is executed when this migration is rolled back

ALTER TABLE `attribute`
 DROP COLUMN `Arg11`,
 DROP COLUMN `Arg12`,
 DROP COLUMN `Arg13`,
 DROP COLUMN `Arg14`,
 DROP COLUMN `Arg15`;

