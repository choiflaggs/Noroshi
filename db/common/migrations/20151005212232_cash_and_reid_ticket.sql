
-- +goose Up
-- SQL in section 'Up' is executed when this migration is applied
CREATE TABLE `exchange_cash_gift` (
  `ID` INT UNSIGNED NOT NULL,
  `Gold` INT UNSIGNED NOT NULL,
  PRIMARY KEY (`ID`));
CREATE TABLE `raid_ticket` (
  `ID` INT UNSIGNED NOT NULL,
  `PlayCount` INT UNSIGNED NOT NULL,
  PRIMARY KEY (`ID`));


-- +goose Down
-- SQL section 'Down' is executed when this migration is rolled back
DROP TABLE `exchange_cash_gift`;
DROP TABLE `raid_ticket`;
