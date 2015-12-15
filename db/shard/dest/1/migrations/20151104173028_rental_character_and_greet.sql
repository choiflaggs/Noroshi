
-- +goose Up
-- SQL in section 'Up' is executed when this migration is applied

ALTER TABLE `player_rental_character`
  CHANGE COLUMN `RentalNum` `RentalNum` int unsigned NOT NULL;

ALTER TABLE `player_status`
  CHANGE COLUMN `GreetedNum` `UnconfirmedGreetedNum` int unsigned NOT NULL,
  ADD COLUMN `LastRentalRewardReceivingNum` tinyint unsigned NOT NULL AFTER `UnconfirmedGreetedNum`,
  ADD COLUMN `LastRentalRewardReceivedAt` int unsigned NOT NULL AFTER `LastRentalRewardReceivingNum`;

-- +goose Down
-- SQL section 'Down' is executed when this migration is rolled back

ALTER TABLE `player_rental_character`
  CHANGE COLUMN `RentalNum` `RentalNum` tinyint unsigned NOT NULL;

ALTER TABLE `player_status`
  CHANGE COLUMN `UnconfirmedGreetedNum` `GreetedNum` smallint unsigned NOT NULL,
  DROP COLUMN `LastRentalRewardReceivingNum`,
  DROP COLUMN `LastRentalRewardReceivedAt`;

