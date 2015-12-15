
-- +goose Up
-- SQL in section 'Up' is executed when this migration is applied
ALTER TABLE `player_stage` 
CHANGE COLUMN `Progress` `Progress` TINYINT UNSIGNED NOT NULL ,
CHANGE COLUMN `PlayCount` `PlayCount` INT UNSIGNED NOT NULL ,
CHANGE COLUMN `ResetCount` `MaxPlayCount` INT UNSIGNED NOT NULL ;


-- +goose Down
-- SQL section 'Down' is executed when this migration is rolled back
ALTER TABLE `player_stage` 
CHANGE COLUMN `Progress` `Progress` INT(1) UNSIGNED NOT NULL DEFAULT 0 ,
CHANGE COLUMN `PlayCount` `PlayCount` INT UNSIGNED NOT NULL DEFAULT 0 ,
CHANGE COLUMN `MaxPlayCount` `ResetCount` INT UNSIGNED NOT NULL DEFAULT 0 ;

