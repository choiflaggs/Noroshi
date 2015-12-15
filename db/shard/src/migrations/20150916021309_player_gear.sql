
-- +goose Up
-- SQL in section 'Up' is executed when this migration is applied
CREATE TABLE `player_character_gear` (
  `ID` INT UNSIGNED NOT NULL,
  `GearID` INT UNSIGNED NOT NULL,
  `PlayerCharacterID` INT UNSIGNED NOT NULL,
  `PromotionLevel` TINYINT UNSIGNED NOT NULL,
  `GearPosition` TINYINT UNSIGNED NOT NULL,
  `Exp` INT UNSIGNED NOT NULL,
  PRIMARY KEY (`ID`),
  UNIQUE (`PlayerCharacterID`, `PromotionLevel`, `GearPosition`));


-- +goose Down
-- SQL section 'Down' is executed when this migration is rolled back
DROP TABLE `player_character_gear`;

