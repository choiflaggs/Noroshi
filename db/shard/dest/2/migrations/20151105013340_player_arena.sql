
-- +goose Up
-- SQL in section 'Up' is executed when this migration is applied
ALTER TABLE `player_arena` 
CHANGE COLUMN `Win` `Win` INT UNSIGNED NOT NULL ,
CHANGE COLUMN `Lose` `Lose` INT UNSIGNED NOT NULL ,
CHANGE COLUMN `DefenseWin` `DefenseWin` INT UNSIGNED NOT NULL ,
CHANGE COLUMN `DefenseLose` `DefenseLose` INT UNSIGNED NOT NULL ,
CHANGE COLUMN `AllHP` `AllHP` INT(10) UNSIGNED NOT NULL ,
CHANGE COLUMN `AllStrength` `AllStrength` INT(10) UNSIGNED NOT NULL ,
CHANGE COLUMN `PlayCount` `PlayCount` INT(10) UNSIGNED NOT NULL ,
CHANGE COLUMN `ResetCount` `ResetCount` INT(10) UNSIGNED NOT NULL ,
CHANGE COLUMN `CoolTime` `CoolTime` INT UNSIGNED NOT NULL ,
CHANGE COLUMN `CreatedAt` `CreatedAt` INT UNSIGNED NOT NULL ,
CHANGE COLUMN `UpdatedAt` `UpdatedAt` INT UNSIGNED NOT NULL ;


-- +goose Down
-- SQL section 'Down' is executed when this migration is rolled back
ALTER TABLE `player_arena` 
CHANGE COLUMN `Win` `Win` INT UNSIGNED NULL ,
CHANGE COLUMN `Lose` `Lose` INT UNSIGNED NULL ,
CHANGE COLUMN `DefenseWin` `DefenseWin` INT UNSIGNED NULL ,
CHANGE COLUMN `DefenseLose` `DefenseLose` INT UNSIGNED NULL ,
CHANGE COLUMN `AllHP` `AllHP` INT(10) UNSIGNED NULL ,
CHANGE COLUMN `AllStrength` `AllStrength` INT(10) UNSIGNED NULL ,
CHANGE COLUMN `PlayCount` `PlayCount` INT(10) UNSIGNED NULL ,
CHANGE COLUMN `ResetCount` `ResetCount` INT(10) UNSIGNED NULL ,
CHANGE COLUMN `CoolTime` `CoolTime` TEXT NOT NULL ,
CHANGE COLUMN `CreatedAt` `CreatedAt` TEXT NOT NULL ,
CHANGE COLUMN `UpdatedAt` `UpdatedAt` TEXT NOT NULL ;

