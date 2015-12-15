
-- +goose Up
-- SQL in section 'Up' is executed when this migration is applied
ALTER TABLE `episode` 
CHANGE COLUMN `AfterEndEpisode` `AfterEndEpisode` TINYINT UNSIGNED NOT NULL ,
CHANGE COLUMN `CharacterID2` `CharacterID2` INT UNSIGNED NULL ,
CHANGE COLUMN `CharacterID3` `CharacterID3` INT UNSIGNED NULL ;
ALTER TABLE `stage` 
CHANGE COLUMN `Type` `Type` TINYINT UNSIGNED NOT NULL ,
CHANGE COLUMN `Stamina` `Stamina` SMALLINT UNSIGNED NOT NULL ,
CHANGE COLUMN `RemainChance` `RemainChance` TINYINT UNSIGNED NOT NULL ,
CHANGE COLUMN `EscapeStamina` `EscapeStamina` SMALLINT UNSIGNED NOT NULL ,
CHANGE COLUMN `MainStage` `MainStage` TINYINT UNSIGNED NOT NULL ;


-- +goose Down
-- SQL section 'Down' is executed when this migration is rolled back
ALTER TABLE `episode` 
CHANGE COLUMN `AfterEndEpisode` `AfterEndEpisode` INT(1) UNSIGNED NOT NULL ,
CHANGE COLUMN `CharacterID2` `CharacterID2` INT UNSIGNED NULL DEFAULT 0 ,
CHANGE COLUMN `CharacterID3` `CharacterID3` INT UNSIGNED NULL DEFAULT 0 ;
ALTER TABLE `noroshi_tomosada`.`stage` 
CHANGE COLUMN `Type` `Type` INT(1) UNSIGNED NOT NULL ,
CHANGE COLUMN `Stamina` `Stamina` INT UNSIGNED NOT NULL ,
CHANGE COLUMN `RemainChance` `RemainChance` INT(3) UNSIGNED NOT NULL ,
CHANGE COLUMN `EscapeStamina` `EscapeStamina` INT UNSIGNED NOT NULL ,
CHANGE COLUMN `MainStage` `MainStage` INT(1) UNSIGNED NOT NULL ;

