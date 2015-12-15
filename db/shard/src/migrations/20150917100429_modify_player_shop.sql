
-- +goose Up
-- SQL in section 'Up' is executed when this migration is applied

ALTER TABLE `player_shop` ADD COLUMN `AppearedAt` int unsigned NOT NULL AFTER `MerchandiseUpdatedAtOnSchedule`;

-- +goose Down
-- SQL section 'Down' is executed when this migration is rolled back

ALTER TABLE `player_shop` DROP COLUMN `AppearedAt`;
