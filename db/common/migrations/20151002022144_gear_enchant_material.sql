
-- +goose Up
-- SQL in section 'Up' is executed when this migration is applied
CREATE TABLE `gear_enchant_material` (
  `ID` INT UNSIGNED NOT NULL,
  `Exp` INT UNSIGNED NOT NULL,
  PRIMARY KEY (`ID`));

ALTER TABLE `drug` 
DROP COLUMN `Type`,
CHANGE COLUMN `ItemID` `ID` INT(10) UNSIGNED NOT NULL ;

ALTER TABLE `gear` 
ADD COLUMN `EnchantExp` INT UNSIGNED NOT NULL AFTER `Gem`;

CREATE TABLE `gear_piece` (
  `ID` INT UNSIGNED NOT NULL,
  `Exp` INT UNSIGNED NOT NULL,
  PRIMARY KEY (`ID`));

ALTER TABLE `soul` 
CHANGE COLUMN `ItemID` `CharacterID` INT(10) UNSIGNED NOT NULL ,
CHANGE COLUMN `Soul` `ConversionSoulCount` INT(3) UNSIGNED NOT NULL ,
DROP PRIMARY KEY,
ADD PRIMARY KEY (`ID`),
DROP INDEX `new_tablecol_UNIQUE` ,
DROP INDEX `ID_UNIQUE` ;

ALTER TABLE `gear_recipe` 
ADD COLUMN `MaterialType` INT(1) UNSIGNED NOT NULL AFTER `CraftItemID`;

ALTER TABLE `character_evolution` 
ADD COLUMN `NecessaryGold` INT UNSIGNED NOT NULL AFTER `Soul`;

-- +goose Down
-- SQL section 'Down' is executed when this migration is rolled back
DROP TABLE `gear_enchant_material`;
DROP TABLE `gear_piece`;l
ALTER TABLE `drug` 
ADD COLUMN `Type` INT(1) UNSIGNED NOT NULL AFTER `ID`,
CHANGE COLUMN `ID` `ItemID` INT(10) UNSIGNED NOT NULL ;

ALTER TABLE `gear` 
DROP COLUMN `EnchantExp`;

ALTER TABLE `soul` 
CHANGE COLUMN `CharacterID` `ItemID` INT(10) UNSIGNED NOT NULL ,
CHANGE COLUMN `NecessaryGold` `Soul` INT(10) UNSIGNED NOT NULL ,
DROP PRIMARY KEY,
ADD PRIMARY KEY (`ID`);

ALTER TABLE `gear_recipe` 
DROP COLUMN `MaterialType`;

ALTER TABLE `character_evolution` 
DROP COLUMN `NecessaryGold`;
