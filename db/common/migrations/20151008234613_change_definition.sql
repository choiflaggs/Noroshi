
-- +goose Up
-- SQL in section 'Up' is executed when this migration is applied
ALTER TABLE `item` 
CHANGE COLUMN `Type` `Type` TINYINT UNSIGNED NOT NULL ,
CHANGE COLUMN `Rarity` `Rarity` TINYINT UNSIGNED NOT NULL ;
ALTER TABLE `drug` 
CHANGE COLUMN `Exp` `Exp` INT UNSIGNED NOT NULL ;
ALTER TABLE `gear` 
CHANGE COLUMN `Level` `Level` SMALLINT UNSIGNED NOT NULL ;
ALTER TABLE `soul` 
CHANGE COLUMN `ConversionSoulCount` `ConversionSoulCount` SMALLINT UNSIGNED NOT NULL ;
ALTER TABLE `character_evolution` 
CHANGE COLUMN `EvolutionLevel` `EvolutionLevel` TINYINT UNSIGNED NOT NULL ,
CHANGE COLUMN `Soul` `Soul` SMALLINT UNSIGNED NOT NULL ;



-- +goose Down
-- SQL section 'Down' is executed when this migration is rolled back
ALTER TABLE `item` 
CHANGE COLUMN `Type` `Type` INT(11) UNSIGNED NOT NULL ,
CHANGE COLUMN `Rarity` `Rarity` INT(11) UNSIGNED NOT NULL ;
ALTER TABLE `drug` 
CHANGE COLUMN `Exp` `Exp` INT(11) UNSIGNED NOT NULL ;
ALTER TABLE `gear` 
CHANGE COLUMN `Level` `Level` INT(10) UNSIGNED NOT NULL ;
ALTER TABLE `noroshi_tomosada`.`soul` 
CHANGE COLUMN `ConversionSoulCount` `ConversionSoulCount` INT(10) UNSIGNED NOT NULL ;
ALTER TABLE `character_evolution` 
CHANGE COLUMN `EvolutionLevel` `EvolutionLevel` INT(3) UNSIGNED NOT NULL ,
CHANGE COLUMN `Soul` `Soul` INT(5) UNSIGNED NOT NULL ;

