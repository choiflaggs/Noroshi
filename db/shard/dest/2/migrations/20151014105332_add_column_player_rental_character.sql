
-- +goose Up
-- SQL in section 'Up' is executed when this migration is applied

ALTER TABLE `player_rental_character`
  ADD COLUMN `RentalNum` tinyint unsigned NOT NULL AFTER `PlayerCharacterID`,
  DROP KEY `i1`,
  ADD UNIQUE KEY `i1` (`PlayerCharacterID`);

-- +goose Down
-- SQL section 'Down' is executed when this migration is rolled back

ALTER TABLE `player_rental_character`
  DROP COLUMN `RentalNum`,
  DROP KEY `i1`,
  ADD UNIQUE KEY `i1` (`PlayerID`,`PlayerCharacterID`);
