
-- +goose Up
-- SQL in section 'Up' is executed when this migration is applied
ALTER TABLE `item` 
CHANGE COLUMN `Reality` `Rarity` INT(11) UNSIGNED NOT NULL,
DROP COLUMN `Price`;



-- +goose Down
-- SQL section 'Down' is executed when this migration is rolled back
ALTER TABLE `item` 
CHANGE COLUMN `Rarity` `Reality` INT(11) NOT NULL ;
ADD COLUMN `Price` INT(11) NOT NULL AFTER `FlavorText`;
