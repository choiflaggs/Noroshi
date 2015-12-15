
-- +goose Up
-- SQL in section 'Up' is executed when this migration is applied
ALTER TABLE `trials` 
CHANGE COLUMN `StartDay` `StartDay` INT UNSIGNED NOT NULL ,
CHANGE COLUMN `EndDay` `EndDay` INT UNSIGNED NOT NULL ;


-- +goose Down
-- SQL section 'Down' is executed when this migration is rolled back
ALTER TABLE `trials` 
CHANGE COLUMN `StartDay` `StartDay` timestamp NOT NULL,,
CHANGE COLUMN `EndDay` `EndDay` timestamp NOT NULL;
