
-- +goose Up
-- SQL in section 'Up' is executed when this migration is applied

ALTER TABLE `login_bonus`
  CHANGE COLUMN `Name` `TextKey` varchar(32) NOT NULL,
  ADD COLUMN `Category` tinyint unsigned NOT NULL AFTER `TextKey`,
  ADD UNIQUE KEY `i1` (`TextKey`);

-- +goose Down
-- SQL section 'Down' is executed when this migration is rolled back

ALTER TABLE `login_bonus`
  CHANGE COLUMN `TextKey` `Name` Text NOT NULL,
  DROP COLUMN `Category`,
  DROP INDEX `i1`;

