
-- +goose Up
-- SQL in section 'Up' is executed when this migration is applied

ALTER TABLE `player_shop`
  CHANGE COLUMN `MerchandiseIDs` `Merchandises` text NOT NULL,
  CHANGE COLUMN `BoughtMerchandiseIDs` `BoughtDisplayNos` text NOT NULL;

-- +goose Down
-- SQL section 'Down' is executed when this migration is rolled back

ALTER TABLE `player_shop`
  CHANGE COLUMN `Merchandises` `MerchandiseIDs` text NOT NULL,
  CHANGE COLUMN `BoughtDisplayNos` `BoughtMerchandiseIDs` text NOT NULL;

